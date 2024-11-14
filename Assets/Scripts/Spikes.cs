using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;  // シーン遷移を使用するために必要

public class Spikes : MonoBehaviour
{
    public float delay = 1.0f;
    public float duration = 0.5f;
    public int hitLimit = 3; // 3回当たったらゲームオーバー

    private Vector3 originalPosition;
    private int hitCount = 0; // 衝突回数をカウント

    void Start()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(SpikeRoutine());
    }

    private IEnumerator SpikeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            transform.localPosition += new Vector3(0, 2, 0); // 上に移動
            yield return new WaitForSeconds(duration);
            transform.localPosition = originalPosition; // 元の位置に戻る
        }
    }

    // Triggerされた時に呼ばれるメソッド
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // プレイヤーが当たった場合
        {
            hitCount++; // 衝突回数をカウント

            if (hitCount >= hitLimit) // 3回当たった場合
            {
                // ゲームオーバーシーンに遷移
                SceneManager.LoadScene("GameOverScene");  // "GameOver"は実際のゲームオーバーシーンの名前に置き換えてください
            }
        }
    }
}
