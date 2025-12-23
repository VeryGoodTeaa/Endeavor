using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Currencies")]
    public float attention;
    public float money;

    [Header("Stats")]
    public float baseClickPower = 1f;
    public float currentClickPower = 1f;
    public float currentPassiveAttention = 0f;
    public float eventChanceMultiplier = 1f;

    [Header("Donation Settings")]
    public float donationCheckInterval = 1f;
    public float baseDonationChance = 0.05f;

    [Tooltip("Сколько кликов нужно сделать для гарантированного доната")]
    public int clicksForGuaranteedDonation = 50;

    [Tooltip("Текущий счетчик кликов (только для чтения)")]
    public int currentClickPity = 0;

    [Header("Game States")]
    public GameState currentState = GameState.Play;

    [Header("Save Settings")]
    public bool autoSaveEnabled = true;
    public float autoSaveInterval = 30f;

    public enum GameState { Play, UpgradeMode }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadGameState();

        StartCoroutine(PassiveLogicRoutine());

        if (autoSaveEnabled)
            StartCoroutine(AutoSaveRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.UpgradeMode)
                UIManager.Instance.SetModeNormal();
            else
                SceneManager.LoadScene("MainMenuScene");
        }
    }

    IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            SaveGame();
        }
    }


    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.money = money;
        data.attention = attention;
        data.currentPity = currentClickPity;

        ClickableObject[] items = FindObjectsOfType<ClickableObject>(true);
        foreach (var item in items)
        {
            if (item.config != null)
            {
                ItemSaveData itemData = new ItemSaveData
                {
                    id = item.config.id,
                    levelIndex = item.currentLevelIndex
                };
                data.items.Add(itemData);
            }
        }

        SaveSystem.Save(data);
    }

    public void LoadGameState()
    {
        if (!SaveSystem.HasSave())
        {
            currentClickPower = baseClickPower;
            return;
        }

        SaveData data = SaveSystem.Load();
        money = data.money;
        attention = data.attention;
        currentClickPity = data.currentPity;

        currentClickPower = baseClickPower;
        currentPassiveAttention = 0f;
        eventChanceMultiplier = 1f;

        ClickableObject[] sceneItems = FindObjectsOfType<ClickableObject>(true);

        foreach (var savedItem in data.items)
        {
            foreach (var sceneItem in sceneItems)
            {
                if (sceneItem.config.id == savedItem.id)
                {
                    sceneItem.ForceSetLevel(savedItem.levelIndex);

                    RecalculateItemBonuses(sceneItem, savedItem.levelIndex);
                    break;
                }
            }
        }
        UIManager.Instance.UpdateCurrencyUI();
    }

    void RecalculateItemBonuses(ClickableObject item, int level)
    {
        for (int i = 0; i < level; i++)
        {
            if (i < item.config.levels.Length)
            {
                var lvl = item.config.levels[i];
                AddGlobalBonuses(lvl.clickPowerBonus, lvl.passiveAttentionBonus, lvl.eventChanceBonus);
            }
        }
    }

    public void HandleClick(Vector3 clickPos)
    {
        float amount = currentClickPower;

        attention += amount;
        UIManager.Instance.ShowClickPopup(amount, clickPos);
        UIManager.Instance.UpdateCurrencyUI();

        currentClickPity++;
        if (currentClickPity >= clicksForGuaranteedDonation)
        {
            ReceiveDonation();
            currentClickPity = 0;
        }
    }

    public void AddGlobalBonuses(float clickBonus, float passiveBonus, float eventBonus)
    {
        currentClickPower += clickBonus;
        currentPassiveAttention += passiveBonus;
        eventChanceMultiplier += eventBonus;

        UIManager.Instance.UpdateCurrencyUI();
    }

    IEnumerator PassiveLogicRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            attention += currentPassiveAttention;
            if (attention < 0) attention = 0;

            CheckPassiveDonationChance();

            UIManager.Instance.UpdateCurrencyUI();
        }
    }

    void CheckPassiveDonationChance()
    {
        float chance = baseDonationChance + (attention / 1000f * 0.01f);

        chance = Mathf.Clamp(chance, 0f, 0.3f);

        if (Random.value < chance)
            ReceiveDonation();
    }

    void ReceiveDonation()
    {
        float donationAmount = Random.Range(10, 100);
        money += donationAmount;

        UIManager.Instance.AddDonationLog(donationAmount);
        UIManager.Instance.UpdateCurrencyUI();
    }

    public void SpendMoney(float amount)
    {
        money -= amount;
        UIManager.Instance.UpdateCurrencyUI();
    }

    private void OnApplicationQuit() => SaveGame();
}