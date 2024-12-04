using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // プレイヤーの移動速度
    [Header("移動の速さ"), SerializeField]
    private float _speed = 3;

    // スプリント時の移動速度
    [Header("スプリント時の速さ"), SerializeField]
    private float _sprintSpeed = 6;

    // ジャンプする瞬間の速度
    [Header("ジャンプする瞬間の速さ"), SerializeField]
    private float _jumpSpeed = 7;

    // 重力加速度
    [Header("重力加速度"), SerializeField]
    private float _gravity = 15;

    // 落下時の速さ制限（無制限の場合はInfinity）
    [Header("落下時の速さ制限（Infinityで無制限）"), SerializeField]
    private float _fallSpeed = 10;

    // 落下の初速
    [Header("落下の初速"), SerializeField]
    private float _initFallSpeed = 2;

    // スタミナの最大値
    [Header("スタミナの最大値"), SerializeField]
    private float _maxStamina = 100f;

    // スタミナ回復速度
    [Header("スタミナ回復速度"), SerializeField]
    private float _staminaRecoveryRate = 10f;

    // スプリント時のスタミナ消費速度
    [Header("スタミナ消費速度"), SerializeField]
    private float _sprintStaminaDrainRate = 10f;

    // プレイヤーのTransformとCharacterControllerの参照
    private Transform _transform;
    private CharacterController _characterController;

    // 入力された移動方向（横・縦）
    private Vector2 _inputMove;

    // 垂直方向の速度（ジャンプや重力による影響）
    private float _verticalVelocity;

    // プレイヤーの回転速度
    private float _turnVelocity;

    // 前回地面に接触しているかどうかの状態
    [SerializeField] private bool _isGroundedPrev;

    // スプリント中かどうか
    private bool _isSprinting;

    // 現在のスタミナ
    private float _currentStamina;

    // スタミナUIのスライダー
    [Header("スタミナUI")]
    [SerializeField]
    private Slider _staminaSlider;

    // ダッシュ音とジャンプ音のAudioClip
    [Header("音声設定")]
    [SerializeField] private AudioClip _sprintSound;
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _WalkSound;

    // プレイヤーのAudioSource
    private AudioSource _audioSource;

    // アニメーターと走り判定用のbool
    private Animator playerAnimator;
    private bool isRun = false;
    private bool isWalk = false;
    private bool isIdle = false;

    // ジャンプ中かどうか
    private bool isJumping = false;

    // スプリント音と歩行音のループ再生のフラグ
    private bool isPlayingSprintSound = false;
    private bool isPlayingWalkSound = false;

    // シフトキーが押されているかどうか
    private bool isHoldingShift = false;

    // ゲームディレクターの参照
    [SerializeField] private GameDirector gameDirector;

    // ポーズマネージャーの参照
    private PauseManager _pauseManager; // PauseManagerの参照を追加

    private void Awake()
    {
        // コンポーネントの初期化
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>(); // AudioSourceの取得
        playerAnimator = GetComponent<Animator>(); // Animatorの取得
        _currentStamina = _maxStamina;

        // スタミナUIの設定
        if (_staminaSlider != null) _staminaSlider.maxValue = _maxStamina;

        // ゲームディレクターの参照
        gameDirector = FindObjectOfType<GameDirector>();

        // PauseManagerの参照を取得
        _pauseManager = FindObjectOfType<PauseManager>();
    }

    private void Update()
    {
        // ポーズ中なら入力を無効化
        if (_pauseManager.IsPaused)
        {
            return; // ポーズ中は入力処理を行わない
        }

        // カウントダウン中は入力を無効にする
        if (gameDirector.isCountdown)
        {
            return;  // カウントダウン中は入力処理を行わない
        }

        // スタミナの管理
        if (_isSprinting)
        {
            // スプリント中にスタミナを減少
            _currentStamina -= _sprintStaminaDrainRate * Time.deltaTime;

            // スタミナがゼロになった場合、スプリントを停止
            if (_currentStamina <= 0)
            {
                _currentStamina = 0;
                _isSprinting = false;
            }

            // スプリント音が再生されていない場合は再生する
            if (!isPlayingSprintSound && _sprintSound != null)
            {
                _audioSource.loop = true;  // 音をループ再生
                _audioSource.clip = _sprintSound;
                _audioSource.Play();
                isPlayingSprintSound = true;
            }
        }
        else
        {
            // スプリントしていない場合、スタミナを回復
            if (_currentStamina < _maxStamina)
            {
                _currentStamina += _staminaRecoveryRate * Time.deltaTime;
                if (_currentStamina > _maxStamina)
                    _currentStamina = _maxStamina;
            }

            // スプリント音が再生されていない場合は停止
            if (isPlayingSprintSound)
            {
                _audioSource.Stop();
                isPlayingSprintSound = false;
            }

            // 歩行中であればウォーク音を再生
            if (_inputMove != Vector2.zero && !isPlayingWalkSound && _WalkSound != null && !_isSprinting)
            {
                _audioSource.loop = true;  // 音をループ再生
                _audioSource.clip = _WalkSound;
                _audioSource.Play();
                isPlayingWalkSound = true;
            }
        }

        // 歩いていない場合、ウォーク音を停止
        if (_inputMove == Vector2.zero && isPlayingWalkSound)
        {
            _audioSource.Stop();
            isPlayingWalkSound = false;
        }

        // 地面に接地しているかどうかをチェック
        var isGrounded = _characterController.isGrounded;

        if (isGrounded && !_isGroundedPrev)
        {
            // 地面に着いた瞬間、初速で少し浮き上がる
            _verticalVelocity = -_initFallSpeed;
            isJumping = false;  // ジャンプが終了した
        }
        else if (!isGrounded)
        {
            // 地面にいない場合、重力の影響で落下する
            _verticalVelocity -= _gravity * Time.deltaTime;

            // 落下速度制限
            if (_verticalVelocity < -_fallSpeed)
                _verticalVelocity = -_fallSpeed;

            isJumping = true;  // ジャンプ中
        }

        _isGroundedPrev = isGrounded;

        // スプリント時の速度調整
        float currentSpeed = _isSprinting ? _sprintSpeed : _speed;

        // 移動ベクトルの計算
        var moveVelocity = new Vector3(
            _inputMove.x * currentSpeed,
            _verticalVelocity,
            _inputMove.y * currentSpeed
        );

        // キャラクターの移動
        var moveDelta = moveVelocity * Time.deltaTime;
        _characterController.Move(moveDelta);

        // 移動入力があればプレイヤーをその方向に回転
        if (_inputMove != Vector2.zero)
        {
            var targetAngleY = -Mathf.Atan2(_inputMove.y, _inputMove.x) * Mathf.Rad2Deg + 90;

            // 角度を0〜360の範囲に制限
            targetAngleY = Mathf.Repeat(targetAngleY, 360f);

            // スムーズに回転させる
            var angleY = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngleY, ref _turnVelocity, 0.1f);
            _transform.rotation = Quaternion.Euler(0, angleY, 0);
        }

        // ジャンプ後の移動アニメーション
        if (!isJumping)
        {
            // 走っているかどうかの判定
            isRun = (_inputMove != Vector2.zero && _isSprinting);

            // 歩いているかどうかの判定
            isWalk = (_inputMove != Vector2.zero && !_isSprinting);  // 走っていない、移動中 → 歩行状態
        }
        else
        {
            // ジャンプ中はアニメーションを「ラン」や「ウォーク」に遷移させない
            isRun = false;
            isWalk = false;

            // ジャンプアニメーションのトリガーを設定
            if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
            {
                playerAnimator.SetTrigger("Jump");
            }
        }

        // アイドル状態の判定
        isIdle = (_inputMove == Vector2.zero && !_isSprinting && !isJumping) ||
                 (isJumping && _inputMove == Vector2.zero && !_isSprinting);  // ジャンプ中でも移動がない場合はアイドル状態

        // ジャンプ終了後にランやウォークに戻る
        if (!isJumping && _inputMove != Vector2.zero)
        {
            if (_isSprinting)
            {
                isRun = true;
            }
            else
            {
                isWalk = true;
            }
        }

        // Animatorに状態を送る
        playerAnimator.SetBool("Run", isRun);
        playerAnimator.SetBool("Walk", isWalk);
        playerAnimator.SetBool("Idle", isIdle);

        // スタミナUIの更新
        if (_staminaSlider != null)
        {
            _staminaSlider.value = _currentStamina;
        }
    }

    // 移動入力の処理
    public void OnMove(InputAction.CallbackContext context)
    {
        if (gameDirector.isCountdown) return;  // カウントダウン中は入力を無効に

        // 移動入力を取得
        _inputMove = context.ReadValue<Vector2>();

        // シフトキーが押されていて移動があればスプリント
        if (isHoldingShift && _inputMove != Vector2.zero)
        {
            _isSprinting = true;
        }
    }

    // ジャンプ入力を処理
    public void OnJump(InputAction.CallbackContext context)
    {
        if (gameDirector.isCountdown) return;  // カウントダウン中は入力を無効に

        if (!context.performed || !_characterController.isGrounded) return;

        // ジャンプ音を再生
        if (_jumpSound != null)
        {
            _audioSource.PlayOneShot(_jumpSound);
        }

        _verticalVelocity = _jumpSpeed;
        isJumping = true; // ジャンプ開始
    }

    // スプリント入力を処理
    // シフトキーが押されたときの処理
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (gameDirector.isCountdown) return;  // カウントダウン中は入力を無効に

        // シフトキーが押されている状態
        isHoldingShift = context.performed;

        // 移動入力があり、シフトキーが押されていればスプリント開始
        if (isHoldingShift && _inputMove != Vector2.zero)
        {
            _isSprinting = true;
        }
        else
        {
            _isSprinting = false;
        }
    }
}
