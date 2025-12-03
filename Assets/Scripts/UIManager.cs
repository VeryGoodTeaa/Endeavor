using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Панели")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject settingsPanel;
    public GameObject savesPanel;
    public GameObject deskUpgradesPanel;
    public GameObject roomUpgradesPanel;

    private GameObject currentPanel;
    private Stack<GameObject> openedPanels = new();

    void Start()
    {
        CloseAllPanels();
        currentPanel = mainMenuPanel;
        currentPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (openedPanels.Count > 0)
                GoBack();
        }
    }

    public void OpenPanel(GameObject newPanel)
    {
        if (currentPanel != null)
        {
            openedPanels.Push(currentPanel);
            currentPanel.SetActive(false);
        }

        newPanel.SetActive(true);
        currentPanel = newPanel;
    }

    public void GoBack()
    {
        if (openedPanels.Count <= 0)
            return;

        currentPanel.SetActive(false);
        GameObject previousPanel = openedPanels.Pop();

        previousPanel.SetActive(true);
        currentPanel = previousPanel;
    }

    private void CloseAllPanels()
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        savesPanel.SetActive(false);
        deskUpgradesPanel.SetActive(false);
        roomUpgradesPanel.SetActive(false);
    }
}