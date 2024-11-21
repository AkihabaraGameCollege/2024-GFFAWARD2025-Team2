using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;  // UIのスライダーを使用するために必要

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移動の速さ"), SerializeField]
    private float _speed = 3;

    [Header("スプリント時の速さ"), SerializeField]
    private float _sprintSpeed = 6;

    [Header("ジャンプする瞬間の速さ"), SerializeField]
    private float _jumpSpeed = 7;

    [Header("重力加速度"), SerializeField]
    private float _gravity = 15;

    [Header("落下時の速さ制限（Infinityで無制限）"), SerializeField]
    private float _fallSpeed = 10;

    [Header("落下の初速"), SerializeField]
    private float _initFallSpeed = 2;

    [Header("スタミナの最大値"), SerializeField]
    private float _maxStamina = 100f;

    [Header("スタミナ回復速度"), SerializeField]
    private float _staminaRecoveryRate = 10f;

    [Header("スタミナ消費速度"), SerializeField]
    private float _sprintStaminaDrainRate = 10f;

    private Transform _transform;
    private CharacterController _characterController;

    private Vector2 _inputMove;
    private float _verticalVelocity;
    private float _turnVelocity;
    private bool _isGroundedPrev;
    private bool _isSprinting;

    // スタミナの現在値
    private float _currentStamina;

    // スライダーUI参照
    [Header("スタミナUI")]
    [SerializeField]
    private Slider _staminaSlider;

    /// <summary>
    /// 移動Action(PlayerInput側から呼ばれる)
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        // 入力値を保持しておく
        _inputMove = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// ジャンプAction(PlayerInput側から呼ばれる)
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        // ボタンが押された瞬間かつ着地している時だけ処理
        if (!context.performed || !_characterController.isGrounded) return;

        // 鉛直上向きに速度を与える
        _verticalVelocity = _jumpSpeed;
    }

    /// <summary>
    /// スプリントAction(PlayerInput側から呼ばれる)
    /// </summary>
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && _currentStamina > 0)
        {
            _isSprinting = true;  // スプリント開始
        }
        else if (context.canceled || _currentStamina <= 0)
        {
            _isSprinting = false;  // スプリント終了
        }
    }

    private void Awake()
    {
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
        _currentStamina = _maxStamina;  // スタミナを初期化
        if (_staminaSlider != null) _staminaSlider.maxValue = _maxStamina;
    }

    private void Update()
    {
        // スタミナの管理
        if (_isSprinting)
        {
            // スプリントしている場合、スタミナが減少
            _currentStamina -= _sprintStaminaDrainRate * Time.deltaTime;
            if (_currentStamina < 0) _currentStamina = 0;

            // スタミナが0になるとスプリントを終了
            if (_currentStamina == 0)
            {
                _isSprinting = false;
            }
        }
        else
        {
            // スプリントしていない場合、スタミナが回復
            _currentStamina += _staminaRecoveryRate * Time.deltaTime;
            if (_currentStamina > _maxStamina) _currentStamina = _maxStamina;
        }

        // スタミナをUIのスライダーに反映
        if (_staminaSlider != null)
        {
            _staminaSlider.value = _currentStamina;
        }

        var isGrounded = _characterController.isGrounded;
        if (isGrounded && !_isGroundedPrev)
        {
            // 着地する瞬間に落下の初速を指定しておく
            _verticalVelocity = -_initFallSpeed;
        }
        else if (!isGrounded)
        {
            // 空中にいるときは、下向きに重力加速度を与えて落下させる
            _verticalVelocity -= _gravity * Time.deltaTime;

            // 落下する速さ以上にならないように補正
            if (_verticalVelocity < -_fallSpeed)
                _verticalVelocity = -_fallSpeed;
        }

        _isGroundedPrev = isGrounded;

        // スプリント時に速度を調整
        float currentSpeed = _isSprinting ? _sprintSpeed : _speed;

        // 操作入力と鉛直方向速度から、現在速度を計算
        var moveVelocity = new Vector3(
            _inputMove.x * currentSpeed,
            _verticalVelocity,
            _inputMove.y * currentSpeed
        );

        // 現在フレームの移動量を移動速度から計算
        var moveDelta = moveVelocity * Time.deltaTime;

        // CharacterControllerに移動量を指定し、オブジェクトを動かす
        _characterController.Move(moveDelta);

        if (_inputMove != Vector2.zero)
        {
            // 移動入力がある場合は、振り向き動作も行う

            // 操作入力からy軸周りの目標角度[deg]を計算
            var targetAngleY = -Mathf.Atan2(_inputMove.y, _inputMove.x) * Mathf.Rad2Deg + 90;

            // イージングしながら次の回転角度[deg]を計算
            var angleY = Mathf.SmoothDampAngle(
                _transform.eulerAngles.y,
                targetAngleY,
                ref _turnVelocity,
                0.1f
            );

            // オブジェクトの回転を更新
            _transform.rotation = Quaternion.Euler(0, angleY, 0);
        }
    }
}