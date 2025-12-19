using UnityEngine;
using TMPro;

public class Popup : MonoBehaviour
{
    public TMP_Text amountText;
    public CanvasGroup canvasGroup;

    public float moveSpeed = 100f;
    public float lifeTime = 1f;

    private float timer;

    public void Setup(int amount)
    {
        amountText.text = "+" + amount.ToString();
        timer = lifeTime;

        canvasGroup.alpha = 1f;
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        timer -= Time.deltaTime;
        if (timer <= 0)
            Destroy(gameObject);
        else
            canvasGroup.alpha = timer / lifeTime;
    }
}