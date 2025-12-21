using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button primaryButton;
    public Button secondaryButton;
    public Button exitButton;

    [Header("Visuals")]
    public TMP_Text primaryBtnText;
    public Sprite bigButtonSprite;
    public Sprite normalButtonSprite;

    [Header("Confirmation")]
    public GameObject confirmationPopup;
    public Button confirmYesButton;
    public Button confirmNoButton;

    private bool hasSaveData;

    private void Start()
    {
        hasSaveData = SaveSystem.HasSave();
        SetupMenu();

        primaryButton.onClick.AddListener(OnPrimaryClick);
        secondaryButton.onClick.AddListener(OnSecondaryNewGameClick);
        exitButton.onClick.AddListener(OnExitClick);

        confirmYesButton.onClick.AddListener(OnConfirmDeleteSave);
        confirmNoButton.onClick.AddListener(() => confirmationPopup.SetActive(false));

        confirmationPopup.SetActive(false);
    }

    void SetupMenu()
    {
        if (hasSaveData)
        {
            primaryBtnText.text = "Продолжить";
            SetButtonSize(primaryButton, true);

            secondaryButton.gameObject.SetActive(true);
            secondaryButton.GetComponentInChildren<TMP_Text>().text = "Начать новую игру";
        }
        else
        {
            primaryBtnText.text = "Начать новую игру";
            SetButtonSize(primaryButton, true);
            secondaryButton.gameObject.SetActive(false);
        }
    }

    void SetButtonSize(Button btn, bool isBig)
    {
        RectTransform rt = btn.GetComponent<RectTransform>();
        if (isBig)
            rt.sizeDelta = new Vector2(400, 100);
        else
            rt.sizeDelta = new Vector2(300, 80);
    }

    void OnPrimaryClick()
    {
        if (hasSaveData)
            LoadGameScene();
        else
            StartNewGame();
    }

    void OnSecondaryNewGameClick() => confirmationPopup.SetActive(true);

    void OnConfirmDeleteSave()
    {
        SaveSystem.DeleteSave();
        StartNewGame();
    }

    void StartNewGame()
    {
        if (SaveSystem.HasSave())
            SaveSystem.DeleteSave();

        LoadGameScene();
    }

    void LoadGameScene() => SceneManager.LoadScene("GameScene");

    void OnExitClick() => Application.Quit();
}
