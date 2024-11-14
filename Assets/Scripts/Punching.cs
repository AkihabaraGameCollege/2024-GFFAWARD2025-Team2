using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // シーン遷移を行うために追加

public class Punching : MonoBehaviour
{
    public float delay = 1.0f; // パンチを発射するまでの遅延時間
    public float duration = 0.5f; // パンチの移動にかかる時間
    public float moveDistance = 28f; // パンチが移動する距離
    public float moveDistance_x = 28f; // 移動する距離
    public string gameOverSceneName = "GameOverScene"; // ゲームオーバーシーンの名前

    private Vector3 originalPosition; // 初期位置
    private LineRenderer lineRenderer; // LineRenderer（パンチの軌跡を描画）
    private bool isPunching = false; // パンチを発射中かどうか
    private int hitCount = 0; // プレイヤーがパンチを受けた回数

    void Start()
    {
        originalPosition = transform.localPosition;

        // LineRenderer の初期設定
        //lineRenderer = gameObject.AddComponent<LineRenderer>();
        //lineRenderer.positionCount = 2;
        //lineRenderer.startWidth = 0.1f;
        //lineRenderer.endWidth = 0.1f;
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };

        // 初期状態でパンチを発射
        StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        while (true)
        {
            if (isPunching)
            {
                // パンチが発射されている間は待機
                yield return null;
                continue;
            }

            isPunching = true;
            yield return new WaitForSeconds(delay); // 遅延時間を待つ

            // パンチの移動先を計算
            Vector3 targetPosition = originalPosition + new Vector3(moveDistance_x, 0, moveDistance);
            float elapsedTime = 0f;

            // スムーズに移動するための処理
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
                //lineRenderer.SetPositions(new Vector3[] { originalPosition, transform.localPosition }); // ラインレンダラーで軌跡を描画
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // パンチを元の位置に戻す
            transform.localPosition = originalPosition;

            // ラインレンダラーの位置を更新
            //lineRenderer.SetPositions(new Vector3[] { originalPosition, originalPosition });

            isPunching = false; // パンチが完了したのでフラグを戻す

            // 3回パンチがプレイヤーに当たったらゲームオーバー
            if (hitCount >= 3)
            {
                LoadGameOverScene(); // ゲームオーバーシーンに遷移
            }
        }
    }

    // プレイヤーにパンチが当たった時に呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーにパンチが当たった場合
        if (other.CompareTag("Player"))
        {
            hitCount++; // 当たった回数をカウント
            Debug.Log("パンチが当たった回数: " + hitCount);

            // もし3回当たったらゲームオーバーシーンに遷移
            if (hitCount >= 3)
            {
                LoadGameOverScene(); // ゲームオーバーシーンに遷移
            }
        }
    }

    // ゲームオーバーシーンに遷移する処理
    private void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName); // シーン遷移
    }

    void OnDisable()
    {
        // スクリプトが無効になった時にラインを消す
        //lineRenderer.positionCount = 0;
    }
}
