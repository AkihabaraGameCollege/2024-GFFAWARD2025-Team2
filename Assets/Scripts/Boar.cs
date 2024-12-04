using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Boar : MonoBehaviour
{
    // イノシシの移動にかかる時間（秒）
    [Header("イノシシの移動にかかる時間"), SerializeField]
    public float duration = 0.5f;
    // イノシシの移動距離（Z軸方向）
    [Header("イノシシの移動距離（Z軸方向）"), SerializeField]
    public float moveDistance = 28f;
    // イノシシの移動距離（X軸方向）
    [Header("イノシシの移動距離（X軸方向）"), SerializeField]
    public float moveDistance_x = 28f;
    // ゲームオーバー時に遷移するシーン名
    [Header("ゲームオーバー時に遷移するシーン名"), SerializeField]
    public string gameOverSceneName = "GameOverScene";

    // イノシシの遅延開始時間（秒）
    [Header("Wave開始時間"), SerializeField]
    public float startTime = 1.0f;
    // イノシシの遅延開始時間（秒）
    [Header("Wave終了時間"), SerializeField]
    public float endTime = 10f;

    // 終了時間（イノシシの開始位置）
    [Header("開始ディレイ時間（イノシシの1サイクルの時間）"), SerializeField]
    public float startDelay = 3.0f;
    // 終了時間（イノシシの終了位置）
    [Header("終了ディレイ時間（イノシシの1サイクルの時間）"), SerializeField]
    public float endDelay = 3.0f;

    // イノシシの初期位置（ゲームオブジェクトのローカル位置）
    private Vector3 originalPosition;
    // イノシシの移動中かどうかを示すフラグ
    private bool isPunching = false;
    // イノシシがプレイヤーに当たった回数
    private int hitCount = 0;

    // 初期化処理
    void Start()
    {
        // イノシシの開始位置を保存
        originalPosition = transform.localPosition;
        // イノシシの移動処理を開始
        StartCoroutine(PunchRoutine());


        // イノシシのコライダーがTriggerとして設定されているか確認
        Collider punchCollider = GetComponent<Collider>();
        if (punchCollider != null)
        {
            // コライダーをTriggerに設定し、物理的な衝突判定を無効化
            punchCollider.isTrigger = true;
        }
    }

    // イノシシの移動アニメーション処理
    private IEnumerator PunchRoutine()
    {
        float waveStartTime = Time.time; // 開始時間を記録

        // イノシシの移動開始までの遅延時間を待つ
        yield return new WaitForSeconds(startTime);  // startDelayを使用


        while (Time.time - waveStartTime < endTime) // endTimeが経過するまで繰り返す
        {
            // すでに移動中の場合は次の移動を待機
            if (isPunching)
            {
                yield return null;
                continue;
            }

            // 移動を開始するフラグを立てる
            isPunching = true;

            // 移動先の最終位置を設定
            Vector3 targetPosition = originalPosition + new Vector3(moveDistance_x, 0, moveDistance);
            float elapsedTime = 0f;

            // 移動アニメーションを実行
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
            // 移動が完了したのでフラグを解除
            isPunching = false;

            // プレイヤーに3回当たった場合、ゲームオーバーシーンへ遷移
            if (hitCount >= 3)
            {
                LoadGameOverScene();
            }

            // 次のイノシシまでの終了遅延時間を待つ
            yield return new WaitForSeconds(endDelay); // endDelayを使用
        }

        Destroy(this.gameObject); // endTimeが経過したら破壊
    }

    // プレイヤーと衝突したときの処理
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
        //SceneManager.LoadScene(gameOverSceneName);
    }


}