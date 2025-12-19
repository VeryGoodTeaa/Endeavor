using UnityEngine;
using UnityEngine.EventSystems;

public enum GameMode
{
    Gameplay,
    UpgradeMainScreen,
    UpgradeExtraScreen
}

public class InteractionManager : MonoBehaviour
{
    public GameMode currentMode = GameMode.Gameplay;

    [Header("Scene References")]
    public GameObject mainScreenRoot;
    public GameObject extraScreenRoot;

    private UpgradableObject hoveredObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetMode(0);
            UIManager.Instance.ResetDropdown();
        }

        HandleMouseInteraction();
    }

    public void SetMode(int modeIndex)
    {
        currentMode = (GameMode)modeIndex;

        switch (currentMode)
        {
            case GameMode.Gameplay:
            case GameMode.UpgradeMainScreen:
                mainScreenRoot.SetActive(true);
                extraScreenRoot.SetActive(false);
                break;
            case GameMode.UpgradeExtraScreen:
                mainScreenRoot.SetActive(false);
                extraScreenRoot.SetActive(true);
                break;
        }
    }

    void HandleMouseInteraction()
    {
        // Игнорируем клики, если курсор над UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        // --- ЛОГИКА ОБВОДКИ (HOVER) ---
        if (currentMode != GameMode.Gameplay)
        {
            if (hit.collider != null && hit.collider.TryGetComponent(out UpgradableObject obj))
            {
                if (hoveredObject != obj)
                {
                    if (hoveredObject != null) hoveredObject.SetHighlight(false);
                    hoveredObject = obj;
                    hoveredObject.SetHighlight(true);
                }

                // Показать подсказку через UIManager
                UpgradeLevelData nextLvl = obj.GetNextLevelData();
                if (nextLvl != null)
                    UIManager.Instance.ShowTooltip(obj.objectName, nextLvl.cost, nextLvl.clickPowerBonus, Input.mousePosition);
                else
                    UIManager.Instance.ShowTooltip(obj.objectName, "MAX LVL", Input.mousePosition);
            }
            else
            {
                if (hoveredObject != null)
                {
                    hoveredObject.SetHighlight(false);
                    hoveredObject = null;
                    UIManager.Instance.HideTooltip();
                }
            }
        }
        else
        {
            if (hoveredObject != null) { hoveredObject.SetHighlight(false); hoveredObject = null; }
            UIManager.Instance.HideTooltip();
        }

        // --- ЛОГИКА КЛИКА ---
        if (Input.GetMouseButtonDown(0))
        {
            if (currentMode == GameMode.Gameplay)
            {
                // Обычный клик - добыча внимания
                GameManager.Instance.AddAttention(GameManager.Instance.clickPower);

                // Визуальный эффект (+10 Attention)
                UIManager.Instance.SpawnClickPopup(mousePos, GameManager.Instance.clickPower);
            }
            else
            {
                if (hoveredObject != null)
                    hoveredObject.Upgrade();
            }
        }
    }
}