using UnityEngine;
using System.Collections;

public class CompassCamera : MonoBehaviour
{
    public Transform target; // 回転の基点となるゲームオブジェクト
    public float rotationSpeed = 90f; // 回転速度（1秒あたりの角度）
    public float rotationDuration = 1f; // 回転完了までの時間
    public AudioClip rotationSound; // 回転時に再生する音
    private AudioSource audioSource; // AudioSourceコンポーネント

    private bool isRotating = false; // 現在回転中かどうかのフラグ

    void Start()
    {
        // AudioSourceコンポーネントを取得
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = rotationSound; // 音を設定
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButton("Fire2") && !isRotating)
        {
            StartCoroutine(RotateAroundTarget(90f));
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetButton("Fire3") && !isRotating)
        {
            StartCoroutine(RotateAroundTarget(-90f));
        }
    }

    private IEnumerator RotateAroundTarget(float angle)
    {
        isRotating = true;
        float totalRotated = 0f;
        float targetRotation = angle;

        // 回転に必要なフレーム数を計算
        float totalRotationTime = Mathf.Abs(targetRotation / rotationSpeed);
        float elapsedTime = 0f;

        // SEを再生
        audioSource.Play();

        while (elapsedTime < totalRotationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalRotationTime; // 0から1に変化する値

            // 線形補間で回転量を計算
            float rotationThisFrame = Mathf.Lerp(0, targetRotation, t);
            transform.RotateAround(target.position, Vector3.up, rotationThisFrame - totalRotated);
            totalRotated = rotationThisFrame;

            yield return null; // 次のフレームを待つ
        }

        // 最後の回転を確実に行う
        transform.RotateAround(target.position, Vector3.up, targetRotation - totalRotated);

        isRotating = false; // 回転フラグをリセット
    }
}
