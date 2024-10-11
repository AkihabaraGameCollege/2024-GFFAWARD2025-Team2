using UnityEngine;
using System.Collections;

public class Punching : MonoBehaviour
{
    public float delay = 1.0f;
    public float duration = 0.5f;
    public float moveDistance = 28f; // 移動する距離

    private Vector3 originalPosition;
    private LineRenderer lineRenderer;

    void Start()
    {
        originalPosition = transform.localPosition;

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
                transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null; // 次のフレームを待つ
            }

            transform.localPosition = originalPosition; // 元の位置に戻る

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
