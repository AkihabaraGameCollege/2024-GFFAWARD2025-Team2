using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;  // UIのImageコンポーネントを使用するために追加

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

    // イメージ非表示設定
    [Header("非表示イメージ設定")]
    [SerializeField] private Image[] imagesToHide;  // 非表示にしたいImageコンポーネントを格納する配列

    // ゲームタイマーに関する設定
    [Header("ゲームタイマー設定")]
    private GameTimer gameTimer;  // GameTimerへの参照

    // カウントダウン中かどうかを示すフラグ
    public bool isCountdown = true;

    void Start()
    {
        // ゲーム開始前にイメージを表示する
        HideImages(false);

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

        // カウントダウン終了後にイメージを非表示にする
        HideImages(true);  // イメージを非表示にする
        countdownText.gameObject.SetActive(false);  // カウントダウンテキストを非表示にする

        // タイムスケールを元に戻してゲーム開始
        Time.timeScale = 1;

        // カウントダウン終了フラグをfalseに
        isCountdown = false;
    }

    // 指定したイメージを非表示/表示にする関数
    void HideImages(bool hide)
    {
        foreach (var image in imagesToHide)
        {
            image.enabled = !hide;  // hideがtrueなら非表示、falseなら表示
        }
    }

    // HPが減少した時に呼ばれる関数
    public void DecreaseHp()
    {
        this.hpGauge.GetComponent<Image>().fillAmount -= 0.1f;  // HPゲージのImageコンポーネントを操作
        var image = this.hpGauge.GetComponent<Image>();
        if (image.fillAmount < 0.1f)
        {
            Debug.Log(image.fillAmount);
            SceneManager.LoadScene("ClearScene");  // HPが0.1未満になったらシーンを遷移
        }
    }
}
