using UnityEngine;
using System.Collections;

public class Punching : MonoBehaviour
{
    public float delay = 1.0f;
    public float duration = 0.5f;
    public float moveDistance = 28f; // 移動する距離

    private Vector3 originalPosition;
    private LineRenderer lineRenderer;
    private Rigidbody rb;

    void Start()
    {
        originalPosition = transform.localPosition;

        // Rigidbodyの初期設定
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true; // 物理の影響を受けないようにする

        // LineRenderer の初期設定
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };

        StartCoroutine(SpikeRoutine());
    }

    private IEnumerator SpikeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Vector3 targetPosition = originalPosition + new Vector3(0, 0, moveDistance);
            float elapsedTime = 0f;

            // スムーズに移動
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                Vector3 newPosition = Vector3.Lerp(originalPosition, targetPosition, t);
                rb.MovePosition(newPosition); // Rigidbodyを使って移動
                elapsedTime += Time.deltaTime;
                yield return null; // 次のフレームを待つ
            }

            rb.MovePosition(originalPosition); // 元の位置に戻る

            // 移動軌道を更新
            lineRenderer.SetPositions(new Vector3[] { originalPosition, targetPosition });
        }
    }

    void OnDisable()
    {
        // スクリプトが無効になった時にラインを消す
        lineRenderer.positionCount = 0;
    }
}
