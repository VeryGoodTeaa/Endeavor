using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class MainClickObject : MonoBehaviour
{
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void PlayClickAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(BounceRoutine());
    }

    IEnumerator BounceRoutine()
    {
        transform.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(0.05f);

        transform.localScale = originalScale * 1.05f;
        yield return new WaitForSeconds(0.05f);

        transform.localScale = originalScale;
    }
}