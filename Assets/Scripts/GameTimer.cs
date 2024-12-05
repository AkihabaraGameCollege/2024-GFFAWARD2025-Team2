using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    // タイマー設定
    [Header("タイマー設定")]
    [SerializeField] private float timeRemaining = 181f;  // 残り時間 (秒)
    [Header("タイマー残り時間")]
    [SerializeField] private float initialTime = 181f;  // 初期の時間 (秒)

    // タイマー表示用設定
    [Header("タイマー表示設定")]
    [SerializeField] private TextMeshProUGUI countdownText;  // タイマー表示用のTextMeshProUGUI

    // ゲームクリア後のシーン遷移設定
    [Header("ゲームクリア後の設定")]
    [SerializeField] private string ClearScene;  // ゲームクリア後に遷移するシーン名

    void Start()
    {
        // PlayerPrefsからタイマー設定を読み込む
        if (PlayerPrefs.HasKey("TimeRemaining"))
        {
            timeRemaining = PlayerPrefs.GetFloat("TimeRemaining");  // 保存された残り時間を取得
        }
        else
        {
            timeRemaining = initialTime;  // 初期設定がある場合はそれを使用
        }

        // タイマーの初期表示を更新
        UpdateTimeDisplay(timeRemaining);
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;  // 1フレームごとに減算
            UpdateTimeDisplay(timeRemaining);
        }
        else
        {
            // タイマーが0になったらクリアシーンに遷移
            timeRemaining = 0;
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

    // ゲームが終了する直前にPlayerPrefsに時間を保存
    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("TimeRemaining", timeRemaining);  // 現在の残り時間を保存
        PlayerPrefs.Save();  // 保存を確定
    }
}
