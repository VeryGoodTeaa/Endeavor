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
    public static InteractionManager Instance;
    public GameMode currentMode = GameMode.Gameplay;

    public GameObject mainScreenRoot;
    public GameObject extraScreenRoot;

    private void Awake() => Instance = this;

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
}