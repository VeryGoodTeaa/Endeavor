using UnityEngine;

public class UpgradableObject : MonoBehaviour
{
    public string objectName;
    public UpgradeLevelData[] levels; // Массив уровней (настраивается в инспекторе)
    public int currentLevelIndex = 0;

    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;
    public Material outlineMaterial; // Материал с шейдером обводки

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
        defaultMaterial = spriteRenderer.material;
    }

    public UpgradeLevelData GetNextLevelData()
    {
        if (currentLevelIndex + 1 < levels.Length)
            return levels[currentLevelIndex + 1];
        return null;
    }

    public void Upgrade()
    {
        UpgradeLevelData nextLvl = GetNextLevelData();
        if (nextLvl != null && GameManager.Instance.TrySpendMoney(nextLvl.cost))
        {
            currentLevelIndex++;
            GameManager.Instance.ApplyUpgradeBonuses(nextLvl);
            UpdateVisuals();
        }
    }

    void UpdateVisuals()
    {
        if (levels[currentLevelIndex].visualState != null)
            spriteRenderer.sprite = levels[currentLevelIndex].visualState;
    }

    public void SetHighlight(bool isActive)
    {
        if (isActive && outlineMaterial != null)
            spriteRenderer.material = outlineMaterial;
        else
            spriteRenderer.material = defaultMaterial;
    }
}