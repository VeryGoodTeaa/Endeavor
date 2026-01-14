using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public void UpdateVisuals()
    {
        targetImage = GetComponent<Image>();
        if (config.levels.Length > 0 && currentLevelIndex <= config.levels.Length)
        {
            if (currentLevelIndex > 0)
            {
                var newSprite = config.levels[currentLevelIndex - 1].visualState;
                targetImage.sprite = newSprite;
            }
            //else
            //    targetImage.sprite = config.levels[0].visualState;
        }
    }

    public void ForceSetLevel(int level)
    {
        currentLevelIndex = level;
        UpdateVisuals();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.currentState == GameManager.GameState.Play)
            GameManager.Instance.HandleClick(eventData.position);
        else if (GameManager.Instance.currentState == GameManager.GameState.UpgradeMode)
            TryUpgrade();
    }

    void TryUpgrade()
    {
        if (currentLevelIndex >= config.levels.Length) return;

        UpgradeLevel nextLvl = config.levels[currentLevelIndex];

        if (GameManager.Instance.money >= nextLvl.cost)
        {
            GameManager.Instance.SpendMoney(nextLvl.cost);
            ApplyUpgrade(nextLvl);
        }
    }

    void ApplyUpgrade(UpgradeLevel lvl)
    {
        currentLevelIndex++;

        GameManager.Instance.AddGlobalBonuses(lvl.clickPowerBonus, lvl.passiveAttentionBonus, lvl.eventChanceBonus);

        UpdateVisuals();
        // ShowTooltip();
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
        if (currentLevelIndex < config.levels.Length)
        {
            UpgradeLevel next = config.levels[currentLevelIndex];
            UIManager.Instance.ShowTooltip(next.cost, next.passiveAttentionBonus, next.clickPowerBonus);
        }
        else
        {
            UIManager.Instance.ShowMaxLevelTooltip();
        }
    }
}