using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Currencies")]
    public float attention;
    public float money;          
    public float moneyProgress;   

    [Header("Balancing - Base Stats")]
    public float baseClickPower = 0.5f;      
    public float baseAttentionGrowth = 1.0f; 
    
    [Header("Balancing - Dynamic")]
    public float currentClickPower;
    public float currentAttentionGrowth;     
    public float currentEventChance = 1f;

    private float streamStabilityMultiplier = 1f; 

    [Header("Game States")]
    public GameState currentState = GameState.Play;

    [Header("Glitch Balance")]
    public float glitchRewardProgress = 35f;   
    public float glitchRewardAttention = 50f;  
    
    public float glitchPenaltyProgress = 100f; 
    public float glitchPenaltyAttention = 5f;  
    public float penaltyDuration = 5f;      

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
        if (autoSaveEnabled) StartCoroutine(AutoSaveRoutine());
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

        SmoothProgressBar.Instance.SetProgress(moneyProgress / 100f);
    }

    public void HandleClick(Vector3 clickPos)
    {
        float amount = currentClickPower; 
        AddProgress(amount);
        UIManager.Instance.ShowClickPopup(amount, clickPos);
    }

    public void AddProgress(float amount)
    {
        moneyProgress += amount;
        
        if (moneyProgress > 100) moneyProgress = 100; 
        if (moneyProgress < 0) moneyProgress = 0;

        UIManager.Instance.UpdateCurrencyUI();

        if (moneyProgress >= 100)
        {
            ReceiveDonation();
            moneyProgress = 0;
        }
    }

    public void ApplyGlitchSuccess(Vector3 pos)
    {
        AddProgress(glitchRewardProgress);
        attention += glitchRewardAttention;

        UIManager.Instance.ShowClickPopup(glitchRewardProgress, pos);
        UIManager.Instance.UpdateCurrencyUI();
    }

    public void ApplyGlitchFail()
    {
        moneyProgress = 0; 
        attention = Mathf.Max(0, attention - glitchPenaltyAttention);

        StartCoroutine(StabilityPenaltyRoutine());

        UIManager.Instance.UpdateCurrencyUI();
    }

    IEnumerator StabilityPenaltyRoutine()
    {
        streamStabilityMultiplier = 0.1f; 
        Debug.Log("Stream Unstable! Audience growth halted.");
        
        yield return new WaitForSeconds(penaltyDuration);
        
        streamStabilityMultiplier = 1f;
        Debug.Log("Stream Stabilized.");
    }

    IEnumerator PassiveLogicRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (currentState == GameState.UpgradeMode) continue;

            float growth = (baseAttentionGrowth + currentAttentionGrowth) * streamStabilityMultiplier;
            
            attention += growth;
            
            if (attention < 0) attention = 0;
            UIManager.Instance.UpdateCurrencyUI();
        }
    }

    void ReceiveDonation()
    {
        float modifier = UnityEngine.Random.Range(0.8f, 1.2f);
        float donationAmount = (attention * 0.1f) * modifier + 10;

        money += donationAmount;

        UIManager.Instance.AddDonationLog(donationAmount);
        UIManager.Instance.UpdateCurrencyUI();
    }

    public void AddGlobalBonuses(float clickBonus, float attentionBonus, float eventBonus)
    {
        currentClickPower += clickBonus;
        currentAttentionGrowth += attentionBonus;
        currentEventChance += eventBonus;

        UIManager.Instance.UpdateCurrencyUI();
    }

    public void LoadGameState()
    {
        if (!SaveSystem.HasSave())
        {
            ResetStats();
            return;
        }

        SaveData data = SaveSystem.Load();
        money = data.money;
        attention = data.attention;

        ResetStats();

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

    void ResetStats()
    {
        currentClickPower = baseClickPower;
        currentAttentionGrowth = 0;
        currentEventChance = 1f;
    }

    // ... Остальные методы (SaveGame, RecalculateItemBonuses, SpendMoney и т.д.) без изменений ...

    IEnumerator AutoSaveRoutine() { while (true) { yield return new WaitForSeconds(autoSaveInterval); SaveGame(); } }
    
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.money = money;
        data.attention = attention;

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

    void RecalculateItemBonuses(ClickableObject item, int level) {
         for (int i = 0; i < level; i++) {
            if (i < item.config.levels.Length) {
                var lvl = item.config.levels[i];
                AddGlobalBonuses(lvl.clickPowerBonus, lvl.passiveAttentionBonus, lvl.eventChanceBonus);
            }
        }
    }

    public void SpendMoney(float amount) { money -= amount; UIManager.Instance.UpdateCurrencyUI(); }
    private void OnApplicationQuit() => SaveGame();
}