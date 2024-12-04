using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameDirector : MonoBehaviour
{
    // ゲームの進行に関する設定
    // HPゲージ設定
    [Header("HPゲージ設定")]
    [SerializeField] private GameObject hpGauge;  // HPゲージのGameObject

    // カウントダウン設定
    [Header("カウントダウン設定")]
    [SerializeField] private float countdownDuration = 3f;  // カウントダウンの時間（秒）

    // UI表示に関する設定
    [Header("カウントダウン表示設定")]
    [SerializeField] private TextMeshProUGUI countdownText;  // カウントダウン表示用のTextMeshProUGUI

    // スプライト非表示設定
    [Header("非表示スプライト設定")]
    [SerializeField] private SpriteRenderer[] spritesToHide;  // 非表示にしたいスプライトを格納する配列

    // ゲームタイマーに関する設定
    [Header("ゲームタイマー設定")]
    private GameTimer gameTimer;  // GameTimerへの参照

    // カウントダウン中かどうかを示すフラグ
    public bool isCountdown = true;

    void Start()
    {
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

        // カウントダウン終了フラグをfalseに
        isCountdown = false;
    }

    // 指定したスプライトを非表示/表示にする関数
    void HideSprites(bool hide)
    {
        foreach (var sprite in spritesToHide)
        {
            sprite.enabled = !hide;  // hideがtrueなら非表示、falseなら表示
        }
    }

    // HPが減少した時に呼ばれる関数
    public void DecreaseHp()
    {
        this.hpGauge.GetComponent<UnityEngine.UI.Image>().fillAmount -= 0.1f;
        var image = this.hpGauge.GetComponent<UnityEngine.UI.Image>();
        if (image.fillAmount < 0.1f)
        {
            Debug.Log(image.fillAmount);
            SceneManager.LoadScene("ClearScene");  // HPが0.1未満になったらシーンを遷移
        }
    }
}
