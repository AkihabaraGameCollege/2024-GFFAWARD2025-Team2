using UnityEngine;

public class Punching : MonoBehaviour
{
    public Transform punchObject; // パンチ位置のオブジェクト
    public float punchForce = 10f;
    private bool isPunched = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isPunched)
        {
            Punch();
        }
    }

    void Punch()
    {
        isPunched = true;
        Rigidbody rb = punchObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.forward * punchForce, ForceMode.Impulse);
        }

        // パンチ後のクールダウン
        Invoke("ResetPunch", 1f);
    }

    void ResetPunch()
    {
        isPunched = false;
    }
}
