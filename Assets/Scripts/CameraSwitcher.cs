using UnityEngine;

public class CompassCamera : MonoBehaviour
{
    public Transform target; // 回転の基点となるゲームオブジェクト
    public float rotationSpeed = 50f; // 回転速度

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            // 左に回転
            RotateAroundTarget(-rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            // 右に回転
            RotateAroundTarget(rotationSpeed * Time.deltaTime);
        }
    }

    private void RotateAroundTarget(float angle)
    {
        // 対象のゲームオブジェクトを中心に回転
        transform.RotateAround(target.position, Vector3.up, angle);
    }
}
