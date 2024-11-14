using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    // Movement Settings: プレイヤーの移動に関する設定
    [Header("Movement Settings")]
    [SerializeField] public float walkSpeed = 5f;      // 歩行時のスピード
    [SerializeField] public float sprintSpeed = 10f;   // ダッシュ時のスピード
    [SerializeField] public float jumpForce = 5f;     // ジャンプの力

    // Ground Detection: 地面判定に関する設定
    [Header("Ground Detection")]
    public LayerMask groundLayer;     // 地面と判定するレイヤーマスク
    [SerializeField] public float groundCheckDistance = 0.1f; // 地面判定の距離

    // Audio: サウンドに関する設定
    [Header("Audio")]
    public AudioClip jumpSound;       // ジャンプ時のサウンド
    public AudioClip sprintSound;     // ダッシュ時のサウンド

    private bool isGrounded;          // プレイヤーが地面に接しているか
    private bool isSprinting;         // プレイヤーがダッシュしているか
    private AudioSource audioSource;  // サウンドを再生するためのAudioSourceコンポーネント

    void Start()
    {
        // Rigidbody コンポーネントを取得して、物理演算を有効にする
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 回転を固定する（キャラクターが回転しないように）

        // AudioSource コンポーネントを取得
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 毎フレーム、入力に基づいて移動、ジャンプ、ダッシュを処理
        HandleMovement();
        HandleJump();
        HandleSprinting();
        CheckGround();  // 地面に接しているか確認
    }

    // プレイヤーの移動処理
    private void HandleMovement()
    {
        // 入力に基づいて移動方向を取得
        float moveX = Input.GetAxisRaw("Horizontal"); // 左右の移動入力 (A/D or 左右矢印)
        float moveZ = Input.GetAxisRaw("Vertical");   // 前後の移動入力 (W/S or 上下矢印)

        // 入力を基に移動方向を作成（x, z 平面での移動）
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized; // y軸は移動に影響しないため 0

        // 移動入力がある場合に、実際に移動処理を行う
        if (moveDirection.magnitude >= 0.1f)
        {
            Move(moveDirection);  // 移動処理
        }
    }

    // プレイヤーの移動処理
    private void Move(Vector3 direction)
    {
        // 移動速度を決定（ダッシュしていればダッシュ速度、そうでなければ歩行速度）
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        // Rigidbody を使って移動する
        rb.MovePosition(rb.position + direction * speed * Time.deltaTime);  // Rigidbody で物理的に移動
    }

    // ジャンプ処理
    private void HandleJump()
    {
        // ジャンプボタンが押され、かつ地面にいる場合にジャンプ
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jump");
            Jump(); // ジャンプ処理
        }
    }

    // ジャンプ処理の実行
    private void Jump()
    {
        // Y軸方向の速度を変更してジャンプする
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);  // X, Z方向の速度は維持しつつ、Y軸方向に力を加える
        audioSource.PlayOneShot(jumpSound); // ジャンプ音を再生
    }

    // ダッシュ処理
    private void HandleSprinting()
    {
        // スプリント入力があればダッシュ開始、無ければダッシュ終了
        bool sprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Fire1");

        if (sprintInput && !isSprinting)
        {
            isSprinting = true;  // ダッシュ状態にする
            audioSource.PlayOneShot(sprintSound); // ダッシュ音を再生
        }
        else if (!sprintInput && isSprinting)
        {
            isSprinting = false;  // ダッシュを終了
        }
    }

    // 地面に接しているかを確認する処理
    private void CheckGround()
    {
        // プレイヤーが地面に接しているかをチェックする
        // Raycastで下方向にレイを飛ばし、指定した距離内に地面があれば地面に接していると判断
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}