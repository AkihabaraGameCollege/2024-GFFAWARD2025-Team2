using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject PauseUI;
    private bool isPaused = false; // ゲームがポーズ中かどうかを追跡
    [SerializeField]
    private string sceneToLoad; // リトライ時にロードするシーン名をインスペクターで設定
    public GameObject PlayerHPUI;
    public GameObject TimerUI;
    public GameObject SutainaUI;
    void Update()
    {
        // キーボードのEscキー
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // ゲームパッドのStartボタン
        if (Input.GetButtonDown("Submit"))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused; // ポーズ状態を反転

        if (isPaused)
        {
            Time.timeScale = 0f; // ポーズ：時間を止める
            PauseUI.SetActive(true); // ポーズメニューを表示
            PlayerHPUI.SetActive(false);
            TimerUI.SetActive(false);
            SutainaUI.SetActive(false);

}
        else
        {
            Time.timeScale = 1f; // 再開：時間を元に戻す
            PauseUI.SetActive(false); // ポーズメニューを表示
            PlayerHPUI.SetActive(true);
            TimerUI.SetActive(true);
            SutainaUI.SetActive(true) ;
        }
    }
    // 再開ボタンが押されたときにポーズを解除
    public void ResumeGame()
    {
        TogglePause(); // ポーズを解除
    }
    // リトライボタンが押されたときにステージをリロード
    public void RetryStage()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad); // インスペクターで設定されたシーン名を使用してロード
            Time.timeScale = 1f; // 時間を元に戻す
        }
        else
        {
            Debug.LogError("シーン名が設定されていません！");
        }
    }
    public void OnClickTitleButton()
    {
        SceneManager.LoadScene("Title Scene");
    }


}
