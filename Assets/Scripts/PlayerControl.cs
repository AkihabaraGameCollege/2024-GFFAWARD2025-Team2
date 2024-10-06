using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour
{
    private Rigidbody rb;

    public float speed = 5f;
    public float jumpForce = 5f; // ジャンプの力
    private bool isGrounded; // 地面にいるかどうか

    // 地面判定のためのレイヤーマスク
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f; // 地面をチェックする距離

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // オブジェクトの回転を固定
    }

    // Update is called once per frame
    void Update()
    {
        // 入力を取得
        float x = Input.GetAxisRaw("Horizontal");

        // 移動方向の計算（左右のみ）
        Vector3 move = transform.right * x;

        // リジットボディを使用して移動
        Move(move);

        // ジャンプ処理
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // 地面判定
        CheckGround();
    }

    private void Move(Vector3 direction)
    {
        // リジットボディに力を加える
        rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
    }

    private void Jump()
    {
        // 上方向に力を加える
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void CheckGround()
    {
        // 地面に接触しているか確認
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // デバッグ用に地面チェックのレイを可視化することも可能（オプション）
        // Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
}
