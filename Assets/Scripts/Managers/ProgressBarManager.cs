using UnityEngine;
using UnityEngine.UI;

public class SmoothProgressBar : MonoBehaviour
{
    public static SmoothProgressBar Instance;

    public Image barImage;
    public float speed = 5f;

    private float targetProgress = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        barImage.fillAmount = Mathf.Lerp(barImage.fillAmount, targetProgress, Time.deltaTime * speed);
    }

    public void SetProgress(float newProgress)
    {
        targetProgress = newProgress;
    }
}