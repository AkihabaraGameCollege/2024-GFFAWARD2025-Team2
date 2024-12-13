using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScene : MonoBehaviour
{
    void Start()
    {
        // ゲーム開始時にマウスカーソルを非表示にする
        Cursor.visible = false;
        // ゲーム中にカーソルがロックされて動かなくする（オプション）
        Cursor.lockState = CursorLockMode.Locked;
    }

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
        SceneManager.LoadScene("GameScene3 2");
    }
    public void OnClickIntroScene()
    {
        SceneManager.LoadScene("IntroScene");
    }
    public void OnClickStage2Button()
    {
        SceneManager.LoadScene("GameScene3 3");
    }
    public void OnClickStage3Button()
    {
        SceneManager.LoadScene("GameScene3 4");
    }
    public void OnClickSelectButton()
    {
        SceneManager.LoadScene("SelectScene");
    }




}
