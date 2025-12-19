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

    public Camera gameCamera;

    private UpgradableObject hoveredUpgradeItem;
    private MainClickObject hoveredClickTarget;

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        SetMode(0);
    //        UIManager.Instance.ResetDropdown();
    //    }

    //    HandleMouseInteraction();
    //}

    public void SetMode(int modeIndex)
    {
        currentMode = (GameMode)modeIndex;
        Debug.Log($"Mode changed to: {currentMode}");

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");
            SetMode(0);
            UIManager.Instance.ResetDropdown();
        }

        HandleMouseInteraction();
    }

    void HandleMouseInteraction()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (gameCamera == null) gameCamera = Camera.main;

        Vector2 mousePos = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (currentMode == GameMode.Gameplay)
        {
            ClearUpgradeHover();

            if (hit.collider != null && hit.collider.TryGetComponent(out MainClickObject target))
            {
                hoveredClickTarget = target;

                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log($"Clicked on: {target.name}");
                    GameManager.Instance.AddAttention(GameManager.Instance.clickPower);
                    target.PlayClickAnimation();
                    UIManager.Instance.SpawnClickPopup(mousePos, GameManager.Instance.clickPower);
                }
            }
            else
            {
                hoveredClickTarget = null;
            }
        }

        else
        {
            if (hit.collider != null && hit.collider.TryGetComponent(out UpgradableObject obj))
            {
                if (hoveredUpgradeItem != obj)
                {
                    ClearUpgradeHover();
                    hoveredUpgradeItem = obj;
                    hoveredUpgradeItem.SetHighlight(true);
                    Debug.Log($"Hovering over upgrade item: {obj.objectName}");
                }

                UpgradeLevelData nextLvl = obj.GetNextLevelData();
                if (nextLvl != null)
                    UIManager.Instance.ShowTooltip(obj.objectName, nextLvl.cost, nextLvl.clickPowerBonus, Input.mousePosition);
                else
                    UIManager.Instance.ShowTooltip(obj.objectName, "MAX LVL", Input.mousePosition);

                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log($"Upgrading: {obj.objectName}");
                    obj.Upgrade();
                }
            }
            else
            {
                ClearUpgradeHover();
            }
        }
    }

    private void ClearUpgradeHover()
    {
        if (hoveredUpgradeItem != null)
        {
            hoveredUpgradeItem.SetHighlight(false);
            hoveredUpgradeItem = null;
        }
        UIManager.Instance.HideTooltip();
    }
}