using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Image))]
public class GlitchVisual : MonoBehaviour, IPointerClickHandler
{
    private GlitchInstance parentInstance;
    private Image myImage;

    private void Awake()
    {
        myImage = GetComponent<Image>();
    }

    public void Initialize(GlitchInstance parent)
    {
        parentInstance = parent;
        myImage.color = Color.white; 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (parentInstance != null)
        {
            parentInstance.RegisterClick();
            
            transform.localScale = Vector3.one * 0.9f;
            StartCoroutine(BounceRoutine());
        }
    }

    IEnumerator BounceRoutine()
    {
        transform.localScale = Vector3.one * 0.8f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = Vector3.one;
    }

    public void SetColor(Color c)
    {
        myImage.color = c;
    }
}