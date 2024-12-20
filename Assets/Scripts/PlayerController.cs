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

    // スプリント中かどうか
    private bool _isSprinting;

    // 現在のスタミナ
    private float _currentStamina;

    // スタミナUIのスライダー
    [Header("スタミナUI")]
    [SerializeField]
    private Slider _staminaSlider;

    // ダッシュ音と歩行音のAudioClip
    [Header("音声設定")]
    [SerializeField] private AudioClip _sprintSound;
    [SerializeField] private AudioClip _WalkSound;

    // プレイヤーのAudioSource
    private AudioSource _audioSource;

    // アニメーターと走り判定用のbool
    private Animator playerAnimator;
    private bool isRun = false;
    private bool isWalk = false;
    private bool isIdle = false;

    // スプリント音と歩行音のループ再生のフラグ
    private bool isPlayingSprintSound = false;
    private bool isPlayingWalkSound = false;

    // シフトキーが押されているかどうか
    private bool isHoldingShift = false;

    // ゲームディレクターの参照
    [SerializeField] private GameDirector gameDirector;

    // ポーズマネージャーの参照
    private PauseManager _pauseManager;

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
        if (_isSprinting && _inputMove != Vector2.zero)
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

        // スプリントしていない場合、速度は通常速度
        float currentSpeed = _isSprinting ? _sprintSpeed : _speed;

        // 移動ベクトルの計算
        Vector3 moveDirection = new Vector3(_inputMove.x, 0, _inputMove.y);
        moveDirection.Normalize();

        // キャラクターの移動
        if (_inputMove != Vector2.zero)
        {
            moveDirection *= currentSpeed;
        }

        // プレイヤーの移動
        _characterController.Move(moveDirection * Time.deltaTime);

        // 移動入力があればプレイヤーをその方向に回転
        if (_inputMove != Vector2.zero)
        {
            var targetAngleY = -Mathf.Atan2(_inputMove.y, _inputMove.x) * Mathf.Rad2Deg + 90;

            // 角度を0〜360の範囲に制限
            targetAngleY = Mathf.Repeat(targetAngleY, 360f);

            // スムーズに回転させる
            _transform.rotation = Quaternion.Euler(0, targetAngleY, 0);
        }

        // スタミナUIの更新
        if (_staminaSlider != null)
        {
            _staminaSlider.value = _currentStamina;
        }

        // 走っているかどうかの判定
        isRun = (_inputMove != Vector2.zero && _isSprinting);

        // 歩いているかどうかの判定
        isWalk = (_inputMove != Vector2.zero && !_isSprinting);  // 走っていない、移動中 → 歩行状態

        // アイドル状態の判定
        isIdle = (_inputMove == Vector2.zero && !_isSprinting);  // 歩いていない場合はアイドル状態

        // Animatorに状態を送る
        playerAnimator.SetBool("Run", isRun);
        playerAnimator.SetBool("Walk", isWalk);
        playerAnimator.SetBool("Idle", isIdle);
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
        else
        {
            _isSprinting = false;
        }
    }

    // スプリント入力を処理
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

    public void OnPause(InputAction.CallbackContext context)
    {
        // カウントダウン中は入力を無効にする
        if (gameDirector.isCountdown)
        {
            return;  // カウントダウン中は入力処理を行わない
        }


        _pauseManager.TogglePause();
    }
}
