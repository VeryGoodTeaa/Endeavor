using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Outline))]
public class ClickableObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UpgradeConfig config;
    public int currentLevelIndex = 0;

    private Image targetImage;
    private Outline outline;

    private void Start()
    {
        targetImage = GetComponent<Image>();
        outline = GetComponent<Outline>();
        outline.enabled = false;

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (currentLevelIndex < config.levels.Length)
            targetImage.sprite = config.levels[currentLevelIndex].visualState;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.currentState == GameManager.GameState.Play)
        {
            float bonus = 0;
            if (currentLevelIndex > 0)
                bonus = config.levels[currentLevelIndex - 1].clickPowerBonus;

            GameManager.Instance.HandleClick(bonus, eventData.position);
        }
        else if (GameManager.Instance.currentState == GameManager.GameState.UpgradeMode)
            TryUpgrade();
    }

    void TryUpgrade()
    {
        if (currentLevelIndex + 1 >= config.levels.Length) return;

        UpgradeLevel nextLvl = config.levels[currentLevelIndex + 1];

        if (GameManager.Instance.money >= nextLvl.cost)
        {
            GameManager.Instance.SpendMoney(nextLvl.cost);
            ApplyUpgrade(nextLvl);
        }
    }

    void ApplyUpgrade(UpgradeLevel lvl)
    {
        currentLevelIndex++;

        GameManager.Instance.currentPassiveAttention += lvl.passiveAttentionBonus;
        GameManager.Instance.eventChanceMultiplier += lvl.eventChanceBonus;

        UpdateVisuals();
        ShowTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.currentState == GameManager.GameState.UpgradeMode)
        {
            outline.enabled = true;
            ShowTooltip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
        UIManager.Instance.HideTooltip();
    }

    void ShowTooltip()
    {
        if (currentLevelIndex + 1 < config.levels.Length)
        {
            UpgradeLevel next = config.levels[currentLevelIndex + 1];
            UIManager.Instance.ShowTooltip(next.cost, next.passiveAttentionBonus, next.clickPowerBonus);
        }
        else
        {
            UIManager.Instance.ShowMaxLevelTooltip();
        }
    }

    public void ForceSetLevel(int level)
    {
        currentLevelIndex = level;
        UpdateVisuals();
    }
}
