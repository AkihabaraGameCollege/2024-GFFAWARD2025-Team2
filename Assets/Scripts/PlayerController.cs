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

    // 無敵時間（秒）
    [Header("無敵時間"), SerializeField]
    private float _invincibilityDuration = 10.0f;

    // 無敵時間のタイマー
    private float _invincibilityTimer = 0.0f;

    // 無敵状態フラグ（通常とパンチ時の無敵）
    private bool _isInvincible = false;
    private bool _isPunchInvincible = false;

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
    private bool _isGroundedPrev;

    // スプリント中かどうか
    private bool _isSprinting;

    // 現在のスタミナ
    private float _currentStamina;

    // スタミナUIのスライダー
    [Header("スタミナUI")]
    [SerializeField]
    private Slider _staminaSlider;

    private void Awake()
    {
        // コンポーネントの初期化
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
        _currentStamina = _maxStamina;

        // スタミナUIの設定
        if (_staminaSlider != null) _staminaSlider.maxValue = _maxStamina;
    }

    private void Update()
    {
        // 無敵状態の処理
        if (_isInvincible || _isPunchInvincible)
        {
            _invincibilityTimer += Time.deltaTime;

            // 無敵時間が終了したら無敵を解除
            if (_invincibilityTimer >= (_isPunchInvincible ? 1.0f : _invincibilityDuration))  // パンチ無敵は1秒
            {
                _isInvincible = false;
                _isPunchInvincible = false;
                _invincibilityTimer = 0.0f;
            }
        }

        // スタミナの管理
        if (_isSprinting)
        {
            // スプリント中にスタミナを減少
            _currentStamina -= _sprintStaminaDrainRate * Time.deltaTime;
            if (_currentStamina < 0) _currentStamina = 0;

            // スタミナが無くなったらスプリントを停止
            if (_currentStamina == 0)
            {
                _isSprinting = false;
                DisableInvincibility();
            }
        }
        else
        {
            // スプリントしていないときはスタミナを回復
            _currentStamina += _staminaRecoveryRate * Time.deltaTime;
            if (_currentStamina > _maxStamina) _currentStamina = _maxStamina;
        }

        // スタミナUIの更新
        if (_staminaSlider != null)
        {
            _staminaSlider.value = _currentStamina;
        }

        // 地面に接地しているかどうかをチェック
        var isGrounded = _characterController.isGrounded;

        if (isGrounded && !_isGroundedPrev)
        {
            // 地面に着いた瞬間、初速で少し浮き上がる
            _verticalVelocity = -_initFallSpeed;
        }
        else if (!isGrounded)
        {
            // 地面にいない場合、重力の影響で落下する
            _verticalVelocity -= _gravity * Time.deltaTime;

            // 落下速度制限
            if (_verticalVelocity < -_fallSpeed)
                _verticalVelocity = -_fallSpeed;
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
            var angleY = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngleY, ref _turnVelocity, 0.1f);
            _transform.rotation = Quaternion.Euler(0, angleY, 0);
        }
    }

    // 移動入力を処理
    public void OnMove(InputAction.CallbackContext context)
    {
        _inputMove = context.ReadValue<Vector2>();
    }

    // ジャンプ入力を処理
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed || !_characterController.isGrounded) return;
        _verticalVelocity = _jumpSpeed;
    }

    // スプリント入力を処理
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && _currentStamina > 0 && _inputMove != Vector2.zero)
        {
            // WASDキーが押されている場合のみスプリント開始
            _isSprinting = true;
        }
        else if (context.canceled || _currentStamina <= 0 || _inputMove == Vector2.zero)
        {
            // スプリント終了（WASDキーが押されていないか、スタミナが無くなった場合）
            _isSprinting = false;
        }
    }

    // 通常の無敵状態を有効化
    private void EnableInvincibility()
    {
        if (!_isInvincible && !_isPunchInvincible)
        {
            _isInvincible = true;
            _invincibilityTimer = 0.0f;
        }
    }

    // 無敵状態を解除
    private void DisableInvincibility()
    {
        if (_isInvincible)
        {
            _isInvincible = false;
            _invincibilityTimer = 0.0f;
        }
    }

    // パンチを受けた場合の無敵を有効化
    public void EnablePunchInvincibility()
    {
        if (!_isPunchInvincible)
        {
            _isPunchInvincible = true;
            _invincibilityTimer = 0.0f;
        }
    }
}
