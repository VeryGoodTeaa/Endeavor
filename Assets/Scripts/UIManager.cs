using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject MainMenuPanel;
    public GameObject GamePanel;
    public GameObject RoomUpgradesPanel;
    public TMP_Dropdown UpgradeTypeDropdown;

    private GameObject currentPanel;
    private Stack<GameObject> openedPanels = new();

    private void Start()
    {
        CloseAllPanels();
        currentPanel = MainMenuPanel;
        currentPanel.SetActive(true);
        UpgradeTypeDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
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
        UpgradeTypeDropdown.value = 0;
        GameManager.Instance.IsUpgradeMode = false;

        if (openedPanels.Count <= 0)
            return;

        currentPanel.SetActive(false);
        GameObject previousPanel = openedPanels.Pop();

        previousPanel.SetActive(true);
        currentPanel = previousPanel;
    }

    private void CloseAllPanels()
    {
        MainMenuPanel.SetActive(false);
        GamePanel.SetActive(false);
        RoomUpgradesPanel.SetActive(false);
    }

    public void OnDropdownChanged(int index)
    {
        switch (index)
        {
            case 0:
                GameManager.Instance.UpgradeDesk();
                break;
            case 1:
                GameManager.Instance.UpgradeRoom();
                break;
            default:
                break;
        }
    }
}