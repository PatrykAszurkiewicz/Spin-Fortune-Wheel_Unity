using UnityEngine;

public class ArrowAnim : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 originalPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBounce()
    {
        // Arrow pulsing scale
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.1f).setLoopPingPong(3);

        // Arrow small movement
        LeanTween.moveLocalY(gameObject, originalPosition.y + 10f, 0.1f).setLoopPingPong(3);
    }
}
