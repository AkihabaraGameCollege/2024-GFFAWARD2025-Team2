using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    void Update()
    {
        // Eキーを押したときの処理
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateCamera(90f); // 右に90度回転
        }

        // Qキーを押したときの処理
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateCamera(-90f); // 左に90度回転
        }
    }

    void RotateCamera(float angle)
    {
        // 現在のカメラの回転を取得
        Vector3 currentRotation = transform.eulerAngles;
        // 新しい回転を計算
        Vector3 newRotation = new Vector3(currentRotation.x, currentRotation.y + angle, currentRotation.z);
        // カメラを回転させる
        transform.eulerAngles = newRotation;
    }
}
