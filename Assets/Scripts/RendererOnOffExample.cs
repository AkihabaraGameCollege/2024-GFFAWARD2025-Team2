using UnityEngine;



public class RendererOnOffExample : MonoBehaviour
{
    // “_–Å‚³‚¹‚é‘ÎÛ
    [SerializeField] private Renderer _target;
    // “_–ÅŽüŠú[s]
    [SerializeField] private float _cycle = 1;

    private double _time;

    private void Update()
    {
        // “à•”Žž‚ðŒo‰ß‚³‚¹‚é
        _time += Time.deltaTime;

        // ŽüŠúcycle‚ÅŒJ‚è•Ô‚·’l‚ÌŽæ“¾
        // 0`cycle‚Ì”ÍˆÍ‚Ì’l‚ª“¾‚ç‚ê‚é
        var repeatValue = Mathf.Repeat((float)_time, _cycle);

        // “à•”Žžtime‚É‚¨‚¯‚é–¾–Åó‘Ô‚ð”½‰f
        _target.enabled = repeatValue >= _cycle * 0.5f;
    }
}