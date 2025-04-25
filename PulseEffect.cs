using UnityEngine;
using System.Collections;

public class PulseEffect : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool isPulsing = false;
    public float speed = 2f;
    public float maxAlpha = 1f;
    public float minAlpha = 0.1f;
    public float fadeOutDuration = 0.5f;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartPulse()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        isPulsing = true;
        canvasGroup.alpha = minAlpha;
        gameObject.SetActive(true);
    }

    public void StopPulse()
    {
        isPulsing = false;
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isPulsing) return;

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * speed, 1));
        canvasGroup.alpha = alpha;
    }
}
