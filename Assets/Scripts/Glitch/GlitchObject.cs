using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GlitchObject : MonoBehaviour, IPointerClickHandler
{
    [Header("Glitch Settings")]
    public float maxDuration = 5f;
    public int clicksToClear = 3;
    public float successReward = 10f;
    public float failurePenalty = 5f;

    [Header("Visual Elements")]
    public Image glitchImage;             
    public RectTransform effectTransform;
    public Color successColor = Color.green;
    public Color failureColor = Color.red;

    public System.Action<GlitchObject> OnGlitchCompleted;

    private ClickableObject parentObject; 
    private float currentDuration;            
    private int currentClicks;               
    private bool isActive = false;           
    private Coroutine timerCoroutine;      
    private Coroutine animationCoroutine;   
    
    private void Start()
    {
        InitializeComponents();
    }

    public void Initialize(ClickableObject parent)
    {
        parentObject = parent;
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (parentObject == null)
        {
            parentObject = GetComponentInParent<ClickableObject>();

            // Если не нашли, ищем через transform.parent
            if (parentObject == null)
            {
                Transform parent = transform.parent;
                while (parent != null && parentObject == null)
                {
                    parentObject = parent.GetComponent<ClickableObject>();
                    parent = parent.parent;
                }
            }
        }

        if (glitchImage == null)
            glitchImage = GetComponent<Image>();

        if (effectTransform == null)
            effectTransform = GetComponent<RectTransform>();

        if (glitchImage != null)
            glitchImage.enabled = false;
    }

    public void ActivateGlitch()
    {
        if (isActive) return;

        isActive = true;
        currentClicks = 0;
        currentDuration = maxDuration;

        if (glitchImage != null)
            glitchImage.enabled = true;

        // Запускаем таймер
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(GlitchTimer());

        // Запускаем визуальную анимацию
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(GlitchAnimation());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive) return;

        currentClicks++;

        if (currentClicks >= clicksToClear)
        {
            ResolveGlitch(true);
        }
        else
        {
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(ClickAnimation());
        }

        eventData.Use();
    }

    private IEnumerator GlitchTimer()
    {
        while (currentDuration > 0 && isActive)
        {
            yield return null;
            currentDuration -= Time.deltaTime;
        }

        if (isActive)
        {
            ResolveGlitch(false);
        }
    }

    private IEnumerator GlitchAnimation()
    {
        if (effectTransform == null) yield break;

        Vector3 originalScale = effectTransform.localScale;
        float animationSpeed = 5f;

        while (isActive)
        {
            float pulse = Mathf.PingPong(Time.time * animationSpeed, 0.2f);
            effectTransform.localScale = originalScale + Vector3.one * pulse;

            yield return null;
        }
    }

    /// <summary>
    /// Анимация при клике
    /// </summary>
    private IEnumerator ClickAnimation()
    {
        if (effectTransform == null) yield break;

        Vector3 originalScale = effectTransform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        // Увеличиваем масштаб
        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            effectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        effectTransform.localScale = targetScale;

        // Возвращаем масштаб
        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            effectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        effectTransform.localScale = originalScale;
    }

    /// <summary>
    /// Завершение помехи (успех или провал)
    /// </summary>
    private void ResolveGlitch(bool success)
    {
        if (!isActive) return;

        isActive = false;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        StartCoroutine(CompletionAnimation(success));
    }

    /// <summary>
    /// Анимация завершения (цвет и исчезновение)
    /// </summary>
    private IEnumerator CompletionAnimation(bool success)
    {
        if (glitchImage != null)
        {
            Color originalColor = glitchImage.color;
            glitchImage.color = success ? successColor : failureColor;

            yield return new WaitForSeconds(0.3f);

            glitchImage.color = originalColor;
        }

        if (success)
            ApplySuccessEffect();
        else
            ApplyFailureEffect();

        yield return StartCoroutine(FadeOut());

        if (glitchImage != null)
            glitchImage.enabled = false;

        if (OnGlitchCompleted != null)
            OnGlitchCompleted(this);
    }

    private IEnumerator FadeOut()
    {
        if (glitchImage == null) yield break;

        Color originalColor = glitchImage.color;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            glitchImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        glitchImage.color = originalColor;
    }

    private void ApplySuccessEffect()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.attention += successReward;
            GameManager.Instance.moneyProgress += successReward;

            //UIManager.Instance.ShowGlitchResult(true, successReward, transform.position);
        }
    }

    private void ApplyFailureEffect()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.attention = Mathf.Max(0, GameManager.Instance.attention - failurePenalty);
            GameManager.Instance.moneyProgress = Mathf.Max(0, GameManager.Instance.moneyProgress - failurePenalty);

            //UIManager.Instance.ShowGlitchResult(false, failurePenalty, transform.position);
        }
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void ForceResolve()
    {
        if (isActive)
            ResolveGlitch(false);
    }
}