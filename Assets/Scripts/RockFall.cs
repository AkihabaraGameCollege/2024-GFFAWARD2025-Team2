using UnityEngine;

public class RockFall : MonoBehaviour
{
    public float fallDelay = 2.0f; // 落下までの遅延時間
    public float fallSpeed = 9.81f; // 落下速度
    [SerializeField]
    private float destroyDelay = 1.0f; // 削除までの遅延時間

    private bool isFalling = false;

    void Start()
    {
        Invoke("StartFalling", fallDelay);
    }

    void Update()
    {
        if (isFalling)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }
    }

    void StartFalling()
    {
        isFalling = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトのタグが「Floor」の場合
        if (collision.gameObject.CompareTag("Floor"))
        {
            // シリアライズフィールドから設定した時間後にオブジェクトを非表示にする
            Invoke("DestroyRock", destroyDelay);
        }
    }

    void DestroyRock()
    {
        Destroy(gameObject); // オブジェクトを削除
    }
}
