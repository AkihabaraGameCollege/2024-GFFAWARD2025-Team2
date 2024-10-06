using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform target; // 回転の中心となるオブジェクト

    void Update()
    {
        // Eキーを押したときの処理
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateCamera(-90f); // 右に90度回転
        }

        // Qキーを押したときの処理
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateCamera(90f); // 左に90度回転
        }
    }

    void RotateCamera(float angle)
    {
        // ターゲットの位置を基にカメラの新しい位置を計算
        Vector3 direction = transform.position - target.position; // ターゲットへの方向
        Quaternion rotation = Quaternion.Euler(0, angle, 0); // Y軸周りの回転
        direction = rotation * direction; // 新しい方向に回転
        transform.position = target.position + direction; // 新しい位置を設定

        // ターゲットを向く
        transform.LookAt(target);
    }
}
