using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [Header("Settings")]
    public ItemType itemType;
    public string itemName;

    [Header("Upgrades")]
    public Sprite[] levelSprites; // Спрайты: 0 - стартовый, 1 - lvl 2...
    public float[] upgradeCosts;  // Цены: [0] цена для перехода на lvl 2
    public float[] buffValues;    // Значение бонуса на каждом уровне
    public int currentLevel = 0;

    [Header("Glitch State")]
    public bool isGlitched = false;
    public Sprite glitchOverlaySprite; // Спрайт "помехи" (красная обводка или шум)
    public GameObject glitchVisualObj; // Дочерний объект со спрайтом помехи

    private SpriteRenderer sr;
    private Color originalColor;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        UpdateSprite();
        if (glitchVisualObj) glitchVisualObj.SetActive(false);
    }

    private void OnMouseEnter()
    {
        // Подсветка
        if (GameManager.Instance.currentMode != GameMode.Playing || isGlitched)
        {
            sr.color = Color.yellow; // Цвет выделения

            if (GameManager.Instance.currentMode != GameMode.Playing)
            {
                // Показать тултип апгрейда
                string desc = GetUpgradeDescription();
                UIManager.Instance.ShowTooltip(transform.position, desc);
            }
        }
    }

    private void OnMouseExit()
    {
        sr.color = originalColor;
        UIManager.Instance.HideTooltip();
    }

    private void OnMouseDown()
    {
        GameMode mode = GameManager.Instance.currentMode;

        // 1. Режим Игры: Починка
        if (mode == GameMode.Playing)
        {
            if (isGlitched)
            {
                FixGlitch();
            }
            // Иначе можно добавить клик-эффект (звук)
        }
        // 2. Режим Апгрейда
        else if ((mode == GameMode.UpgradeDesk && IsDeskItem()) ||
                 (mode == GameMode.UpgradeRoom && IsRoomItem()))
        {
            TryUpgrade();
        }
    }

    private void FixGlitch()
    {
        isGlitched = false;
        if (glitchVisualObj) glitchVisualObj.SetActive(false);
        GlitchManager.Instance.ReportGlitchFixed();

        // Бонус за починку
        // GameManager.Instance.money += 5; 
    }

    public void TriggerGlitch()
    {
        if (isGlitched) return;
        isGlitched = true;
        if (glitchVisualObj) glitchVisualObj.SetActive(true);
    }

    private void TryUpgrade()
    {
        if (currentLevel >= upgradeCosts.Length) return; // Макс уровень

        float cost = upgradeCosts[currentLevel];
        if (GameManager.Instance.money >= cost)
        {
            GameManager.Instance.money -= cost;
            currentLevel++;
            ApplyBuffs();
            UpdateSprite();
            // Обновить тултип сразу после покупки
            UIManager.Instance.ShowTooltip(transform.position, GetUpgradeDescription());
        }
    }

    private void ApplyBuffs()
    {
        // Логика применения бонусов в зависимости от типа предмета
        float val = buffValues[currentLevel - 1]; // Берем значение только что полученного уровня

        switch (itemType)
        {
            case ItemType.Mouse: // Лучше клик
                GameManager.Instance.clickPower += val;
                break;
            case ItemType.Plant: // Лучше пассив
                GameManager.Instance.passiveProgress += val;
                break;
                // Добавить логику для остальных
        }
    }

    private void UpdateSprite()
    {
        if (currentLevel < levelSprites.Length && levelSprites[currentLevel] != null)
        {
            sr.sprite = levelSprites[currentLevel];
        }
    }

    private string GetUpgradeDescription()
    {
        if (currentLevel >= upgradeCosts.Length) return "MAX LEVEL";

        float cost = upgradeCosts[currentLevel];
        float nextBuff = buffValues[currentLevel];

        string buffTxt = itemType == ItemType.Plant ? "Пассив/сек" : "Клик";
        return $"{itemName} Lvl {currentLevel + 1}\nЦена: {cost}$\n+{nextBuff} {buffTxt}";
    }

    private bool IsDeskItem() => itemType != ItemType.Sofa && itemType != ItemType.Plant;
    private bool IsRoomItem() => !IsDeskItem();
}