using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StatusManager : MonoBehaviour
{
    [SerializeField] GameObject MainObject; //このスクリプトをアタッチするオブジェクト
    [SerializeField] int hp = 1;             //hp現在値
    [SerializeField] int maxHp = 1;          //いずれmaxHp利用する際に使用

    [SerializeField] GameObject destroyEffect;  //撃破エフェクト
    [SerializeField] GameObject damageEffect;   //被弾エフェクト

    [SerializeField] string TagName;            //当たり判定となるタグ
    public GameObject[] lifeArray = new GameObject[3]; //HPのゲームオブジェクト
    private int lifePoint = 3;
    [SerializeField]
    private string GameOverScene;
    [SerializeField]
    private bool invincibleTime = false; //無敵時間
    // Update is called once per frame
    void Update()
    {

        //hpが0以下なら、撃破エフェクトを生成してMainを破壊
        if (hp <= 0)
        {
            DestoryMainObject();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
        //当たり判定のタグネームと一致していればダメージ処理呼び出し
        if (other.CompareTag(TagName))
        {
            Debug.Log("Hit2");
            if (!invincibleTime)
            {
                Damage();
            }
        }
    }
    private void Damage()
    {
        Debug.Log("Hit3");
        hp--;
        lifeArray[lifePoint - 1].SetActive(false);//HPのゲームオブジェクトを非表示にさせる
        lifePoint--;//要素を一つ－する
        //var effect = Instantiate(damageEffect);
        //effect.transform.position = transform.position;
        invincibleTime = true;
        StartCoroutine(Damageinvinble());

    }
    private void DestoryMainObject()
    {
        Debug.Log("Hit4");
        hp = 0;
        //var effect = Instantiate(destroyEffect);
        //effect.transform.position = transform.position;
        //Destroy(effect, 5);
        SceneManager.LoadScene(GameOverScene); // インスペクターで設定されたシーン名を使用してロード

    }
    private IEnumerator Damageinvinble()
    {
        yield return new WaitForSeconds(3f);
        invincibleTime = false;
        Debug.Log("muteki");

    }
}
