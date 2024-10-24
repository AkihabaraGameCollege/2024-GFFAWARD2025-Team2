using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlawController : MonoBehaviour
{
    private int touchCount = 0;
    public GameObject objectPrefab; // 生成するオブジェクトのプレハブ
    public Vector3 spawnAreaMin; // 生成範囲の最小点
    public Vector3 spawnAreaMax; // 生成範囲の最大点

    private GameObject currentObject;

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        if (currentObject == null)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );
            currentObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // プレイヤーがオブジェクトに触れたかチェック
        {
            touchCount++;
            Destroy(currentObject); // ゲームオブジェクトを削除
            currentObject = null; // 削除後のオブジェクト参照をクリア

            if (touchCount >= 10)
            {
                SceneManager.LoadScene("ClearScene"); // 10回触れたらシーン遷移
            }
            else
            {
                SpawnObject(); // 新しいオブジェクトを生成
            }
        }
    }
}