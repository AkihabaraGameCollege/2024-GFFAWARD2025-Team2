using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Rigidbodyコンポーネントを必ず持つことを要求する
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;   // プレイヤーのRigidbodyコンポーネントを保持する変数

    public float speed = 5f;    // 通常の移動速度
    public float sprintSpeed = 10f; // 走るときの速度
    [SerializeField] private float jumpForce = 5f; // ジャンプの初速度
    [SerializeField] private float jumpAcceleration = 5f; // ジャンプ中の加速度
    private bool isGrounded; // プレイヤーが地面にいるかどうかを示すフラグ

    public LayerMask groundLayer;   // 地面のレイヤーを指定するための変数
    public float groundCheckDistance = 0.1f; // 地面をチェックするための距離

    private Vector3 jumpVelocity; // ジャンプの速度

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbodyコンポーネントを取得
        rb.freezeRotation = true; // オブジェクトの回転を固定し、物理的な回転を防ぐ
    }

    void Update()
    {
        // 入力に基づいて移動方向を取得
        float x = Input.GetAxisRaw("Horizontal"); // 左右の移動入力
        //float z = Input.GetAxisRaw("Vertical"); // 前後の移動入力

        Vector3 move = new Vector3(x, 0/*, z*/);    // 入力に基づく移動ベクトルを作成

        Move(move); // 移動処理を呼び出す

        // ジャンプボタンが押され、かつ地面にいる場合、ジャンプ処理を実行
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        ApplyJumpAcceleration(); // ジャンプ中の加速度を適用
        CheckGround();  // 地面のチェックを実行
    }

    private void Move(Vector3 direction)
    {
        // カメラの向きを基に前方と右方を計算
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // Y成分をゼロにして水平面での移動を制限
        forward.y = 0;
        right.y = 0;

        // 正規化して方向ベクトルを取得
        forward.Normalize();
        right.Normalize();

        // 入力方向に基づいて最終的な移動ベクトルを計算
        Vector3 moveDirection = (right * direction.x + forward * direction.z).normalized;

        // 現在の速度を決定（シフトキーが押されているかによって
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

        // Rigidbodyを使って新しい位置に移動
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        jumpVelocity = Vector3.up * jumpForce;
        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity.y, rb.velocity.z);
    }

    private void ApplyJumpAcceleration()
    {
        if (!isGrounded && rb.velocity.y < 0)
        {
            rb.AddForce(Vector3.down * jumpAcceleration, ForceMode.Acceleration);
        }
    }

    private void CheckGround()
    {
        // Raycastを使ってプレイヤーが地面に接触しているかを確認
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spikes"))
        {
            // ゲームオーバーシーンに遷移
            SceneManager.LoadScene("GameOver");
        }
    }
}
