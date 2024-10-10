using UnityEngine;
using System.Collections;

public class CompassCamera : MonoBehaviour
{
    public Transform target; // 回転の基点となるゲームオブジェクト
    public float rotationSpeed = 90f; // 回転速度（1秒あたりの角度）
    public float rotationDuration = 1f; // 回転完了までの時間

    private bool isRotating = false; // 現在回転中かどうかのフラグ

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            StartCoroutine(RotateAroundTarget(90f));
        }
        else if (Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            StartCoroutine(RotateAroundTarget(-90f));
        }
    }

    private IEnumerator RotateAroundTarget(float angle)
    {
        isRotating = true;
        float totalRotated = 0f;
        float targetRotation = angle;

        // 各フレームでの回転量を計算
        float step = rotationSpeed * Time.deltaTime;

        while (totalRotated < Mathf.Abs(targetRotation))
        {
            // このフレームでの回転量を決定
            float rotationThisFrame = Mathf.Min(step, Mathf.Abs(targetRotation - totalRotated));
            transform.RotateAround(target.position, Vector3.up, Mathf.Sign(targetRotation) * rotationThisFrame);
            totalRotated += rotationThisFrame;

            yield return null; // 次のフレームを待つ
        }

        isRotating = false; // 回転フラグをリセット
    }
}
