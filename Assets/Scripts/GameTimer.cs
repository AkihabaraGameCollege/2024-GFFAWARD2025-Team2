using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    // タイマー設定
    [Header("タイマー設定")]
    [SerializeField] private float timeRemaining = 181f;  // 残り時間 (秒)
    [Header("初期タイマー設定")]
    [SerializeField] private float initialTime = 181f;  // 初期の時間 (秒)

    // タイマー表示用設定
    [Header("タイマー表示設定")]
    [SerializeField] private TextMeshProUGUI countdownText;  // タイマー表示用のTextMeshProUGUI

    // ゲームクリア後のシーン遷移設定
    [Header("ゲームクリア後の設定")]
    [SerializeField] private string ClearScene;  // ゲームクリア後に遷移するシーン名

    private float startTime;  // ゲーム開始時刻を保存

    void Start()
    {
        // 初期タイマー設定を使用
        timeRemaining = initialTime;

        // ゲーム開始時刻を記録
        startTime = Time.time;

        // タイマーの初期表示を更新
        UpdateTimeDisplay(timeRemaining);
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            // 経過時間を計算
            timeRemaining = initialTime - (Time.time - startTime);

            // 0未満にならないように制御
            if (timeRemaining < 0) timeRemaining = 0;

            UpdateTimeDisplay(timeRemaining);
        }
        else
        {
            // タイマーが0になったらクリアシーンに遷移
            UpdateTimeDisplay(timeRemaining);
            SceneManager.LoadScene(ClearScene);
        }
    }

    // 時間表示を更新するメソッド
    void UpdateTimeDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);  // 分を計算
        int seconds = Mathf.FloorToInt(time % 60);  // 秒を計算
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);  // フォーマットして表示
    }
}
