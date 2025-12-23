using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class DonationItem : MonoBehaviour
{
    [Header("Settings")]
    public float visibleTime = 4f;
    public float fadeDuration = 1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        yield return new WaitForSeconds(visibleTime);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void ForceFadeOutAndDestroy()
    {
        StopAllCoroutines();
        StartCoroutine(FastFadeOut());
    }

    IEnumerator FastFadeOut()
    {
        float fastDuration = 0.2f;
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < fastDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fastDuration);
            yield return null;
        }
        Destroy(gameObject);
    }
}