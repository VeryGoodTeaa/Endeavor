using UnityEngine;
using TMPro;

public class PopupEffect : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 150f;
    public float lifeTime = 1.0f;
    public float maxRotationAngle = 20f;

    private TMP_Text tmpText;
    private float startAlpha;
    private float timer;

    private void Awake()
    {
        tmpText = GetComponentInChildren<TMP_Text>();
        startAlpha = tmpText.color.a;

        float randomZ = Random.Range(-maxRotationAngle, maxRotationAngle);
        transform.localRotation = Quaternion.Euler(0, 0, randomZ);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        timer += dt;

        transform.Translate(Vector3.up * moveSpeed * dt, Space.World);
        float progress = timer / lifeTime;

        if (tmpText != null)
        {
            Color c = tmpText.color;
            c.a = Mathf.Lerp(startAlpha, 0f, progress);
            tmpText.color = c;
        }

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}