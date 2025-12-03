using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject savesPanel;
    public GameObject settingsPanel;
    public GameObject deskUpgradesPanel;
    public GameObject roomUpgradesPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    private void HideAll()
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(false);
        savesPanel.SetActive(false);
        settingsPanel.SetActive(false);
        deskUpgradesPanel.SetActive(false);
        roomUpgradesPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        HideAll();
        mainMenuPanel.SetActive(true);
    }

    public void ShowGame()
    {
        HideAll();
        gamePanel.SetActive(true);
    }

    public void ShowSettings()
    {
        HideAll();
        settingsPanel.SetActive(true);
    }

    public void ShowSaves()
    {
        HideAll();
        savesPanel.SetActive(true);
    }
}