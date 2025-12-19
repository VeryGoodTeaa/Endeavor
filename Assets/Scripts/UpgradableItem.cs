using UnityEngine;
using UnityEngine.EventSystems; // Важно!
using UnityEngine.UI;

public class UIUpgradableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string objectName;
    public UpgradeLevelData[] levels;
    public int currentLevelIndex = 0;

    private Image targetImage; // Вместо SpriteRenderer теперь Image

    // Ссылка на материал обводки (для UI нужен специальный UI-шейдер, 
    // либо можно просто менять цвет картинки Image.color)
    public Material outlineMaterial;

    void Start()
    {
        targetImage = GetComponent<Image>();
        UpdateVisuals();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Логика работает только в режиме улучшений
        if (InteractionManager.Instance.currentMode != GameMode.Gameplay)
        {
            // Включаем подсветку (например, через материал или цвет)
            if (outlineMaterial != null) targetImage.material = outlineMaterial;
            else targetImage.color = Color.yellow; // Простая подсветка цветом

            // Показываем тултип
            ShowTooltip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetImage.material = null;
        targetImage.color = Color.white;
        UIManager.Instance.HideTooltip();   
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InteractionManager.Instance.currentMode != GameMode.Gameplay)
        {
            Upgrade();
        }
    }

    void Upgrade()
    {
        // Логика улучшения (такая же, как была раньше)
        UpgradeLevelData nextLvl = GetNextLevelData();
        if (nextLvl != null && GameManager.Instance.TrySpendMoney(nextLvl.cost))
        {
            currentLevelIndex++;
            GameManager.Instance.ApplyUpgradeBonuses(nextLvl);
            UpdateVisuals();

            // Обновляем тултип сразу после покупки, чтобы цена изменилась
            ShowTooltip();
        }
    }

    void UpdateVisuals()
    {
        if (levels[currentLevelIndex].visualState != null)
            targetImage.sprite = levels[currentLevelIndex].visualState;
    }

    UpgradeLevelData GetNextLevelData()
    {
        if (currentLevelIndex + 1 < levels.Length) return levels[currentLevelIndex + 1];
        return null;
    }

    void ShowTooltip()
    {
        UpgradeLevelData nextLvl = GetNextLevelData();
        if (nextLvl != null)
            UIManager.Instance.ShowTooltip(objectName, nextLvl.cost, nextLvl.clickPowerBonus, Input.mousePosition);
        else
            UIManager.Instance.ShowTooltip(objectName, "MAX", Input.mousePosition);
    }
}