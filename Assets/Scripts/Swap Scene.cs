using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScene : MonoBehaviour
{
    //シーンの遷移を指定する。
    public void OnClickTitleButton()
    {
        SceneManager.LoadScene("Title Scene");
    }
    public void OnClickExitButton()
    {
        Application.Quit();
        Debug.Log("終わり");
    }
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("GameScene3 1");
    }
    public void OnClickIntroScene()
    {
        SceneManager.LoadScene("IntroScene");
    }



}
