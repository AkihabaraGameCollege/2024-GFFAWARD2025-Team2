using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI‚ðŽg‚¤‚Ì‚Å–Y‚ê‚¸‚É’Ç‰Á
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    GameObject hpGauge;

    void Start()
    {
        this.hpGauge = GameObject.Find("hpGauge");
    }

    public void DecreaseHp()
    {
        this.hpGauge.GetComponent<Image>().fillAmount -= 0.1f;
        var image = this.hpGauge.GetComponent<Image>();
        if(image.fillAmount < 0.8f)
        {
            Debug.Log(image.fillAmount);
            SceneManager.LoadScene("ClearScene");
        }
    }
}