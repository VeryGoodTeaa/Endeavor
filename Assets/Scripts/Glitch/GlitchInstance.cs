using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class GlitchInstance : MonoBehaviour
{
    [Header("Settings")]
    public GameObject glitchPrefab;
    public int clicksToFix = 5;
    public float duration = 4.0f;

    [Header("Visuals")]
    public Color successColor = Color.green;
    public Color failColor = Color.red;

    private int currentClicks;
    private bool isActive = false;
    private Coroutine glitchRoutine;
    
    private GameObject currentVisualObject;
    private GlitchVisual currentVisualScript;
    private RectTransform myRect;

    public bool IsActive => isActive;

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }

    public void ActivateGlitch(int requiredClicks, float time)
    {
        if (isActive) return;

        clicksToFix = requiredClicks;
        duration = time;
        currentClicks = 0;
        isActive = true;

        SpawnVisual();

        if (glitchRoutine != null) StopCoroutine(glitchRoutine);
        glitchRoutine = StartCoroutine(GlitchTimerRoutine());
    }

    void SpawnVisual()
    {
        currentVisualObject = Instantiate(glitchPrefab, transform);
        currentVisualScript = currentVisualObject.GetComponent<GlitchVisual>();
        currentVisualScript.Initialize(this);

        RectTransform glitchRect = currentVisualObject.GetComponent<RectTransform>();
        
        float width = myRect.rect.width;
        float height = myRect.rect.height;

        float paddingX = glitchRect.rect.width / 2;
        float paddingY = glitchRect.rect.height / 2;

        float randomX = Random.Range(-(width / 2) + paddingX, (width / 2) - paddingX);
        float randomY = Random.Range(-(height / 2) + paddingY, (height / 2) - paddingY);

        glitchRect.anchoredPosition = new Vector2(randomX, randomY);
    }

    public void RegisterClick()
    {
        if (!isActive) return;

        currentClicks++;

        if (currentClicks >= clicksToFix)
            Succeed();
    }

    private void Succeed()
    {
        isActive = false;
        StopCoroutine(glitchRoutine);
        
        GameManager.Instance.ApplyGlitchSuccess(transform.position);

        if (currentVisualScript != null)
            StartCoroutine(EndEffectRoutine(successColor));
    }

    private void Fail()
    {
        isActive = false;
        
        GameManager.Instance.ApplyGlitchFail();

        if (currentVisualScript != null)
            StartCoroutine(EndEffectRoutine(failColor));
    }

    private IEnumerator GlitchTimerRoutine()
    {
        float timer = duration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            
            if (currentVisualObject != null)
            {
                currentVisualObject.transform.localPosition += (Vector3)Random.insideUnitCircle * 2f;
                yield return null;
                if(currentVisualObject != null)
                     currentVisualObject.transform.localPosition -= (Vector3)Random.insideUnitCircle * 2f; 
            }
            else
            {
                yield return null;
            }
        }
        
        Fail();
    }

    private IEnumerator EndEffectRoutine(Color targetColor)
    {
        if (currentVisualScript != null)
            currentVisualScript.SetColor(targetColor);
        
        yield return new WaitForSeconds(0.5f);

        if (currentVisualObject != null)
            Destroy(currentVisualObject);
    }
}