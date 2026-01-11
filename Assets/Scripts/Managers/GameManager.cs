using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    Playing,        // Активная игра, клики, помехи
    UpgradeDesk,    // Пауза, улучшение стола
    UpgradeRoom     // Пауза, улучшение комнаты
}

public enum ItemType
{
    Mouse, Keyboard, Microphone, MonitorDevice, Sofa, Plant, Decoration
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Resources")]
    public float money = 0;
    public float attention = 100; // 0 to 100+
    public float currentEtherProgress = 0;
    public float maxEtherProgress = 100;
    public float moneyPerEther = 50; // Награда за заполнение бара

    [Header("Stats")]
    public float clickPower = 5f;
    public float passiveProgress = 1f;
    public float attentionDecayRate = 0.5f; // Падение внимания при помехах

    [Header("UI References")]
    public Slider progressSlider;
    public TMP_Text moneyText;
    public TMP_Text attentionText;
    public GameObject deskContainer;
    public GameObject roomContainer;
    public TMP_Dropdown modeDropdown;

    [Header("Game State")]
    public GameMode currentMode = GameMode.Playing;
    public int activeGlitchesCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
        SwitchMode(0); // Start in Playing mode

        // Подписка на изменение Dropdown
        modeDropdown.onValueChanged.AddListener(SwitchMode);
    }

    private void Update()
    {
        if (currentMode == GameMode.Playing)
        {
            HandleGameLoop();
        }
    }

    private void HandleGameLoop()
    {
        // Влияние помех
        float penalty = activeGlitchesCount * 0.2f; // Штраф к пассивному доходу 20% за глитч
        float actualPassive = passiveProgress * (1f - penalty);

        if (actualPassive < 0) actualPassive = 0;

        // Пассивный набор
        AddProgress(actualPassive * Time.deltaTime);

        // Логика внимания
        if (activeGlitchesCount > 0)
        {
            attention -= attentionDecayRate * activeGlitchesCount * Time.deltaTime;
        }
        else
        {
            // Медленное восстановление внимания, если все чисто
            if (attention < 100) attention += 1f * Time.deltaTime;
        }

        // Ограничение внимания
        attention = Mathf.Clamp(attention, 0, 200);

        UpdateUI();
    }

    // Вызывается при клике по фону/экрану (не по предметам)
    public void OnMainClick()
    {
        if (currentMode != GameMode.Playing) return;

        // Если слишком много помех, клик может не работать или давать меньше
        if (activeGlitchesCount >= 3) return;

        AddProgress(clickPower);
    }

    private void AddProgress(float amount)
    {
        currentEtherProgress += amount;
        if (currentEtherProgress >= maxEtherProgress)
        {
            // Эфир завершен -> Награда
            float attentionBonus = (attention / 100f); // Если внимание 120, бонус x1.2
            float reward = moneyPerEther * attentionBonus;

            money += reward;
            currentEtherProgress = 0;

            // Здесь можно вызвать эффект "Донат получен"
            UIManager.Instance.ShowDonation("Эфир", reward);
        }
    }

    // Метод переключения режимов из Dropdown
    public void SwitchMode(int modeIndex)
    {
        currentMode = (GameMode)modeIndex;

        switch (currentMode)
        {
            case GameMode.Playing:
                deskContainer.SetActive(true);
                roomContainer.SetActive(false);
                Time.timeScale = 1; // Игра идет
                break;
            case GameMode.UpgradeDesk:
                deskContainer.SetActive(true);
                roomContainer.SetActive(false);
                Time.timeScale = 0; // Пауза для апгрейда
                break;
            case GameMode.UpgradeRoom:
                deskContainer.SetActive(false);
                roomContainer.SetActive(true);
                Time.timeScale = 0; // Пауза
                break;
        }
    }

    private void UpdateUI()
    {
        moneyText.text = $"{Mathf.Floor(money)}$";
        attentionText.text = $"👁 {Mathf.Floor(attention)}";
        progressSlider.value = currentEtherProgress / maxEtherProgress;
    }
}