using UnityEngine;
using System.Collections;

public class Punching1 : MonoBehaviour
{
    public float delay1 = 1.0f;
    public float duration1 = 0.5f;
    public float moveDistance1 = 28f; // 移動する距離

    private Vector3 originalPosition1;
    private LineRenderer lineRenderer1;
    private Rigidbody rb1;

    void Start()
    {
        originalPosition1 = transform.localPosition;

        // Rigidbodyの初期設定
        rb1 = gameObject.AddComponent<Rigidbody>();
        rb1.isKinematic = true; // 物理の影響を受けないようにする

        // LineRenderer の初期設定
        lineRenderer1 = gameObject.AddComponent<LineRenderer>();
        lineRenderer1.positionCount = 2;
        lineRenderer1.startWidth = 0.1f;
        lineRenderer1.endWidth = 0.1f;
        lineRenderer1.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };

        StartCoroutine(SpikeRoutine1());
    }

    private IEnumerator SpikeRoutine1()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay1);
            Vector3 targetPosition = originalPosition1 + new Vector3(moveDistance1, 0, 0);
            float elapsedTime = 0f;

            // スムーズに移動
            while (elapsedTime < duration1)
            {
                float t = elapsedTime / duration1;
                Vector3 newPosition = Vector3.Lerp(originalPosition1, targetPosition, t);
                rb1.MovePosition(newPosition); // Rigidbodyを使って移動
                elapsedTime += Time.deltaTime;
                yield return null; // 次のフレームを待つ
            }

            rb1.MovePosition(originalPosition1); // 元の位置に戻る

            // 移動軌道を更新
            lineRenderer1.SetPositions(new Vector3[] { originalPosition1, targetPosition });
        }
    }

    void OnDisable()
    {
        // スクリプトが無効になった時にラインを消す
        lineRenderer1.positionCount = 0;
    }
}
