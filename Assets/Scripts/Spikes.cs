using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour
{
    public float delay = 1.0f;
    public float duration = 0.5f;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(SpikeRoutine());
    }

    private IEnumerator SpikeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            transform.localPosition += new Vector3(0, 1, 0); // è„Ç…à⁄ìÆ
            yield return new WaitForSeconds(duration);
            transform.localPosition = originalPosition; // å≥ÇÃà íuÇ…ñﬂÇÈ
        }
    }
}
