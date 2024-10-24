using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class BossHPController : MonoBehaviour
{
    [SerializeField] GameObject AttackPoint;
    [SerializeField] GameObject player;
    Vector3 myPos;
    [SerializeField] float xValue = 3f;
    [SerializeField]float  zValue = 3f;
    // Start is called before the first frame update
    void Start()
    {
        myPos = player.transform.position;
            }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // 監督スクリプトにプレイヤと衝突したことを伝える
            GameObject director = GameObject.Find("GameDirector");
            director.GetComponent<GameDirector>().DecreaseHp();

            //弱点を次の位置に生成して、自らを破壊
            GameObject go = Instantiate(AttackPoint);
            int px = Random.Range(-6, 7);
            int pz = Random.Range(-6, 7);
            go.transform.position = new Vector3(px * xValue, 0, pz * zValue) + myPos + new Vector3(20f, 0, 0);
            Destroy(gameObject);
        }
    }
}
