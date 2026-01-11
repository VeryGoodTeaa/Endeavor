using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GlitchEffect : MonoBehaviour, IPointerClickHandler
{
    [Header("Glitch Settings")]
    public float maxDuration = 5f;           // Максимальное время до дебафа
    public int clicksToClear = 3;             // Количество кликов для очистки помехи
    public float successReward = 10f;         // Бонус за успешное устранение помехи
    public float failurePenalty = 5f;         // Штраф за неудачное устранение помехи

    [Header("Visual Elements")]
    public Image glitchImage;                 // Изображение помехи
    public RectTransform effectTransform;     // Трансформ для анимации
    public Color successColor = Color.green;  // Цвет при успешном устранении
    public Color failureColor = Color.red;    // Цвет при провале

    // Делегат для уведомления о завершении помехи
    public System.Action<GlitchEffect> OnGlitchCompleted;

    private ClickableObject parentObject;     // Родительский кликабельный объект
    private float currentDuration;            // Текущее время до дебафа
    private int currentClicks;                // Текущее количество кликов
    private bool isActive = false;            // Активна ли помеха
    private Coroutine timerCoroutine;         // Корутина таймера
    private Coroutine animationCoroutine;     // Корутина анимации

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
        // Получаем родительский объект типа ClickableObject, если не был установлен через Initialize
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

        // Инициализируем компоненты
        if (glitchImage == null)
            glitchImage = GetComponent<Image>();

        if (effectTransform == null)
            effectTransform = GetComponent<RectTransform>();

        // Скрываем изображение по умолчанию
        if (glitchImage != null)
            glitchImage.enabled = false;
    }

    /// <summary>
    /// Активирует помеху на заданном объекте
    /// </summary>
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

    /// <summary>
    /// Обработка клика по помехе
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive) return;

        // Увеличиваем счетчик кликов
        currentClicks++;
        
        // Проверяем, достигли ли нужного количества кликов
        if (currentClicks >= clicksToClear)
        {
            // Успешно устранили помеху
            ResolveGlitch(true);
        }
        else
        {
            // Продолжаем анимацию клика
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(ClickAnimation());
        }

        // Предотвращаем передачу клика дальше
        eventData.Use();
    }

    /// <summary>
    /// Таймер обратного отсчета до дебафа
    /// </summary>
    private IEnumerator GlitchTimer()
    {
        while (currentDuration > 0 && isActive)
        {
            yield return null;
            currentDuration -= Time.deltaTime;
        }

        // Если время вышло и помеха еще активна - применяем дебаф
        if (isActive)
        {
            ResolveGlitch(false);
        }
    }

    /// <summary>
    /// Визуальная анимация помехи
    /// </summary>
    private IEnumerator GlitchAnimation()
    {
        if (effectTransform == null) yield break;

        Vector3 originalScale = effectTransform.localScale;
        float animationSpeed = 5f;

        while (isActive)
        {
            // Пульсирующий эффект
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

        // Останавливаем корутины
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

        // Применяем визуальный эффект завершения
        StartCoroutine(CompletionAnimation(success));
    }

    /// <summary>
    /// Анимация завершения (цвет и исчезновение)
    /// </summary>
    private IEnumerator CompletionAnimation(bool success)
    {
        if (glitchImage != null)
        {
            // Меняем цвет в зависимости от результата
            Color originalColor = glitchImage.color;
            glitchImage.color = success ? successColor : failureColor;

            // Ждем немного, чтобы показать цвет
            yield return new WaitForSeconds(0.3f);

            // Возвращаем оригинальный цвет
            glitchImage.color = originalColor;
        }

        // Применяем эффект к игре
        if (success)
        {
            ApplySuccessEffect();
        }
        else
        {
            ApplyFailureEffect();
        }

        // Плавное исчезновение
        yield return StartCoroutine(FadeOut());

        // Сбрасываем состояние
        if (glitchImage != null)
            glitchImage.enabled = false;

        // Уведомляем подписчиков о завершении помехи
        if (OnGlitchCompleted != null)
        {
            OnGlitchCompleted(this);
        }
    }

    /// <summary>
    /// Плавное исчезновение
    /// </summary>
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

    /// <summary>
    /// Применение эффекта при успехе
    /// </summary>
    private void ApplySuccessEffect()
    {
        if (GameManager.Instance != null)
        {
            // Увеличиваем внимание и деньги как награду
            GameManager.Instance.attention += successReward;
            
            // Показываем визуальный эффект успеха
            UIManager.Instance.ShowGlitchResult(true, successReward, transform.position);
        }
    }

    /// <summary>
    /// Применение эффекта при провале
    /// </summary>
    private void ApplyFailureEffect()
    {
        if (GameManager.Instance != null)
        {
            // Уменьшаем внимание и деньги как штраф
            GameManager.Instance.attention = Mathf.Max(0, GameManager.Instance.attention - failurePenalty);
            GameManager.Instance.moneyProgress = Mathf.Max(0, GameManager.Instance.moneyProgress - failurePenalty);
            
            // Показываем визуальный эффект провала
            UIManager.Instance.ShowGlitchResult(false, failurePenalty, transform.position);
        }
    }

    /// <summary>
    /// Проверка, активна ли помеха
    /// </summary>
    public bool IsActive()
    {
        return isActive;
    }

    /// <summary>
    /// Принудительное завершение помехи
    /// </summary>
    public void ForceResolve()
    {
        if (isActive)
        {
            ResolveGlitch(false); // При принудительном завершении считаем как провал
        }
    }
}