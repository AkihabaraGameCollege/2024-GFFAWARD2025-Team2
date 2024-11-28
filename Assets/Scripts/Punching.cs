using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Punching : MonoBehaviour
{
    // パンチの遅延時間（秒）
    public float delay = 1.0f;
    // パンチのアニメーションの時間（秒）
    public float duration = 0.5f;
    // パンチの移動距離（Z軸方向）
    public float moveDistance = 28f;
    // パンチの移動距離（X軸方向）
    public float moveDistance_x = 28f;
    // ゲームオーバー時に遷移するシーン名
    public string gameOverSceneName = "GameOverScene";

    // パンチの開始位置（ゲームオブジェクトのローカル位置）
    private Vector3 originalPosition;
    // パンチが現在実行中かどうかを示すフラグ
    private bool isPunching = false;
    // パンチが当たった回数
    private int hitCount = 0;

    // 初期化処理
    void Start()
    {
        // パンチの開始位置を保存
        originalPosition = transform.localPosition;
        // パンチ処理を開始
        StartCoroutine(PunchRoutine());

        // 確認: パンチのコライダーが Trigger として設定されていること
        Collider punchCollider = GetComponent<Collider>();
        if (punchCollider != null)
        {
            // パンチのコライダーをTriggerとして設定することで、物理的な衝突判定を無効にする
            punchCollider.isTrigger = true;
        }
    }

    // パンチアニメーションのルーチン
    private IEnumerator PunchRoutine()
    {
        while (true)
        {
            // パンチ中は次のパンチを開始しない
            if (isPunching)
            {
                yield return null;
                continue;
            }

            // パンチ処理を開始するフラグを立てる
            isPunching = true;

            // 次のパンチまでの遅延時間を待つ
            yield return new WaitForSeconds(delay);

            // パンチの最終位置を設定
            Vector3 targetPosition = originalPosition + new Vector3(moveDistance_x, 0, moveDistance);
            float elapsedTime = 0f;

            // パンチの移動アニメーションを進める
            while (elapsedTime < duration)
            {
                // アニメーションの進行具合（0～1の範囲）
                float t = elapsedTime / duration;
                // 線形補間で位置を計算して移動させる
                transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 最後に元の位置に戻す
            transform.localPosition = originalPosition;
            // パンチ処理が完了したことを示すフラグを解除
            isPunching = false;

            // パンチが3回当たった場合、ゲームオーバーシーンに遷移する
            if (hitCount >= 3)
            {
                LoadGameOverScene();
            }
        }
    }

    // プレイヤーとパンチが衝突したときの処理
    private void OnTriggerEnter(Collider other)
    {
        // 衝突したオブジェクトが「Player」タグのオブジェクトであるかを確認
        if (other.CompareTag("Player"))
        {
            // ヒットカウントをインクリメント
            hitCount++;
            Debug.Log("パンチが当たった回数: " + hitCount);

            // ヒットカウントが3回に達したらゲームオーバーシーンに遷移
            if (hitCount >= 3)
            {
                LoadGameOverScene();
            }
        }
    }

    // ゲームオーバーシーンに遷移する
    private void LoadGameOverScene()
    {
        // ゲームオーバーシーンをロード
        SceneManager.LoadScene(gameOverSceneName);
    }
}
