using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    // UIのTextコンポーネントを参照
    public Text countdownText;

    // カウントダウンの開始時間（秒）
    [SerializeField] private float timeRemaining = 180f;  // 3分 = 180秒
    [SerializeField]
    private string ClearScene;
    void Start()
    {
        // 初期テキストの表示
        UpdateTimeDisplay(timeRemaining);
    }

    void Update()
    {
        // 時間が残っている場合はカウントダウン
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeDisplay(timeRemaining);
        }
        else
        {
            // カウントダウン終了時の処理
            timeRemaining = 0;
            UpdateTimeDisplay(timeRemaining);
            // 終了時に何かしたい場合はここに処理を追加
            // 例: Debug.Log("Time's up!");
            SceneManager.LoadScene(ClearScene);
        }
    }

    // 時間表示を更新するメソッド
    void UpdateTimeDisplay(float time)
    {
        // 時間を分と秒に分割
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // 2桁表示にするためにフォーマット
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
