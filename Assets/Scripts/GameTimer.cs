using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;  // タイマー用のTextMeshProUGUI
    [SerializeField] private float timeRemaining = 181f;  // 3分1秒 (181秒)
    [SerializeField] private string ClearScene;  // ゲームクリア後に遷移するシーン名

    void Start()
    {
        // シーン遷移時にtimeRemainingをリセット
        timeRemaining = 181f;  // 3分1秒

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
}
