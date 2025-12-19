using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainClickButton : MonoBehaviour, IPointerClickHandler
{
    private Button btn;
    private Vector3 originalScale;

    void Start()
    {
        btn = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InteractionManager.Instance.currentMode == GameMode.Gameplay)
        {
            PerformClick();
        }
    }

    void PerformClick()
    {
        GameManager.Instance.AddAttention(GameManager.Instance.clickPower);

        StopAllCoroutines();
        StartCoroutine(AnimateButton());

        UIManager.Instance.SpawnClickPopup(Input.mousePosition, GameManager.Instance.clickPower);
    }

    System.Collections.IEnumerator AnimateButton()
    {
        transform.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(0.05f);
        transform.localScale = originalScale;
    }
}