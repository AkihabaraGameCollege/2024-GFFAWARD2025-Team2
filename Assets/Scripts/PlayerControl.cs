using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour
{
    private Rigidbody rb;

    public float speed = 5f;

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
    }

    private void Move(Vector3 direction)
    {
        // リジットボディに力を加える
        rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
    }
}
