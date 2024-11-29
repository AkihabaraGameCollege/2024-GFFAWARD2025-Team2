using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameDirector : MonoBehaviour
{
    GameObject hpGauge;
    public TextMeshProUGUI countdownText;  // TextMeshProUGUIを使う
    public float countdownDuration = 3f;  // カウントダウンの時間（秒）
    public SpriteRenderer[] spritesToHide;  // 非表示にしたいスプライトを格納する配列
    private GameTimer gameTimer;  // GameTimerへの参照

    void Start()
    {
        this.hpGauge = GameObject.Find("hpGauge");

        // ゲーム開始前にスプライトを表示する
        HideSprites(false);

        // GameTimerコンポーネントを取得
        gameTimer = FindObjectOfType<GameTimer>();

        // ゲーム開始前にカウントダウンを実行
        StartCoroutine(StartCountdown());
    }

    // ゲーム開始前のカウントダウンを管理するコルーチン
    IEnumerator StartCountdown()
    {
        // カウントダウンの最初の秒数を表示
        countdownText.gameObject.SetActive(true);

        // カウントダウンの文字列を順番に表示
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();  // 3, 2, 1 を表示
            Time.timeScale = 0;  // タイムスケールを0にしてゲームを一時停止
            yield return new WaitForSecondsRealtime(1);  // 実際の時間で1秒待機
        }

        // テキストを少し左にずらす
        RectTransform rectTransform = countdownText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x - 2, rectTransform.anchoredPosition.y);

        // 「GO!!」を表示
        countdownText.text = "GO!!";

        yield return new WaitForSecondsRealtime(1);  // 実際の時間で1秒待機

        // カウントダウン終了後にスプライトを非表示にする
        HideSprites(true);  // スプライトを非表示にする
        countdownText.gameObject.SetActive(false);  // カウントダウンテキストを非表示にする

        // タイムスケールを元に戻してゲーム開始
        Time.timeScale = 1;
    }

    // 指定したスプライトを非表示/表示にする関数
    void HideSprites(bool hide)
    {
        foreach (var sprite in spritesToHide)
        {
            sprite.enabled = !hide;  // hideがtrueなら非表示、falseなら表示
        }
    }

    public void DecreaseHp()
    {
        this.hpGauge.GetComponent<UnityEngine.UI.Image>().fillAmount -= 0.1f;
        var image = this.hpGauge.GetComponent<UnityEngine.UI.Image>();
        if (image.fillAmount < 0.1f)
        {
            Debug.Log(image.fillAmount);
            SceneManager.LoadScene("ClearScene");
        }
    }
}
