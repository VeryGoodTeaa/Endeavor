using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class MonitorController : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable]
    public struct StreamMode
    {
        public string modeName;
        public Sprite screenImage;
        [Header("Bonuses (1.0 = normal, 1.2 = +20%)")]
        public float clickMultiplier;
        public float attentionMultiplier;
        public float donationMultiplier;
    }

    [Header("Config")]
    public StreamMode[] modes;

    private int currentModeIndex = 0;
    private Image myImage;
    private GlitchInstance myGlitch;

    private void Start()
    {
        myImage = GetComponent<Image>();
        myGlitch = GetComponent<GlitchInstance>();

        if (modes.Length > 0)
            ApplyMode(0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.currentState == GameManager.GameState.UpgradeMode) return;

        if (myGlitch != null && myGlitch.IsActive) return;

        SwitchToNextMode();
    }

    void SwitchToNextMode()
    {
        if (modes.Length == 0) return;

        currentModeIndex++;
        if (currentModeIndex >= modes.Length)
            currentModeIndex = 0;

        ApplyMode(currentModeIndex);
        
        StartCoroutine(AnimateSwitch());
    }

    void ApplyMode(int index)
    {
        StreamMode mode = modes[index];

        myImage.sprite = mode.screenImage;

        GameManager.Instance.SetMonitorBonuses(
            mode.clickMultiplier, 
            mode.attentionMultiplier, 
            mode.donationMultiplier
        );
        
        UIManager.Instance.ShowClickPopup($"Monitor theme changed:\n" +
            $"Click: x{mode.clickMultiplier}\n" +
            $"Attention: x{mode.attentionMultiplier}\n" +
            $"Doantion: x{mode.donationMultiplier}", transform.position, true, 50);
    }

    System.Collections.IEnumerator AnimateSwitch()
    {
        transform.localScale = Vector3.one * 0.95f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = Vector3.one;
    }
}