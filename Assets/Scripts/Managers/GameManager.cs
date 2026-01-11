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

    [Header("Stats")]
    public float baseClickPower = 1f;
    public float currentClickPower = 1f;
    public float currentPassiveProgress = 0f;
    public float eventChanceMultiplier = 1f;

    [Header("Glitch Spawn Settings")]
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 25f;
    public float activeGlitchLimit = 2;

    [Header("Glitch Prefab")]
    public GameObject glitchPrefab;

    private List<GlitchObject> activeGlitches = new List<GlitchObject>();
    private Coroutine glitchSpawnCoroutine;

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

        SmoothProgressBar.Instance.SetProgress(moneyProgress / 100f);
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

        currentClickPower = baseClickPower;
        currentPassiveProgress = 0f;
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

        moneyProgress += amount;
        UIManager.Instance.ShowClickPopup(amount, clickPos);
        UIManager.Instance.UpdateCurrencyUI();

        if (moneyProgress >= 100)
        {
            ReceiveDonation();
            moneyProgress = 0;
        }
    }

    public void AddGlobalBonuses(float clickBonus, float passiveBonus, float eventBonus)
    {
        currentClickPower += clickBonus;
        currentPassiveProgress += passiveBonus;
        eventChanceMultiplier += eventBonus;

        UIManager.Instance.UpdateCurrencyUI();
    }

    private IEnumerator PassiveLogicRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            moneyProgress += currentPassiveProgress;
            if (moneyProgress < 0) moneyProgress = 0;

            UIManager.Instance.UpdateCurrencyUI();
        }
    }

    public void StartGlitchSpawning()
    {
        if (glitchSpawnCoroutine != null)
        {
            StopCoroutine(glitchSpawnCoroutine);
        }

        glitchSpawnCoroutine = StartCoroutine(GlitchSpawnRoutine());
    }

    public void StopGlitchSpawning()
    {
        if (glitchSpawnCoroutine != null)
        {
            StopCoroutine(glitchSpawnCoroutine);
            glitchSpawnCoroutine = null;
        }
    }

    private IEnumerator GlitchSpawnRoutine()
    {
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (activeGlitches.Count >= activeGlitchLimit)
                continue;

            AttemptToSpawnGlitch();
        }
    }

    private void AttemptToSpawnGlitch()
    {
        ClickableObject[] clickableObjects = FindObjectsOfType<ClickableObject>();

        if (clickableObjects.Length == 0)
        {
            Debug.LogWarning("Не найдено ни одного ClickableObject для спавна помехи");
            return;
        }

        List<ClickableObject> availableObjects = new List<ClickableObject>();
        foreach (var obj in clickableObjects)
        {
            GlitchObject existingGlitch = obj.GetComponentInChildren<GlitchObject>();
            if (existingGlitch == null || !existingGlitch.IsActive())
            {
                availableObjects.Add(obj);
            }
        }

        if (availableObjects.Count == 0)
        {
            Debug.Log("Нет доступных объектов для спавна помехи");
            return;
        }

        ClickableObject targetObject = availableObjects[UnityEngine.Random.Range(0, availableObjects.Count)];

        SpawnGlitchOnObject(targetObject);
    }

    private void SpawnGlitchOnObject(ClickableObject targetObject)
    {
        if (glitchPrefab == null)
        {
            Debug.LogError("Glitch prefab не назначен!");
            return;
        }

        GameObject glitchInstance = Instantiate(glitchPrefab, targetObject.transform);

        RectTransform glitchRect = glitchInstance.GetComponent<RectTransform>();
        if (glitchRect != null)
        {
            glitchRect.anchorMin = Vector2.zero;
            glitchRect.anchorMax = Vector2.one;
            glitchRect.offsetMin = Vector2.zero;
            glitchRect.offsetMax = Vector2.zero;
        }

        GlitchObject glitchEffect = glitchInstance.GetComponent<GlitchObject>();
        if (glitchEffect == null)
            glitchEffect = glitchInstance.AddComponent<GlitchObject>();

        glitchEffect.Initialize(targetObject);
        glitchEffect.ActivateGlitch();
        activeGlitches.Add(glitchEffect);
    }

    public void ClearAllGlitches()
    {
        for (int i = activeGlitches.Count - 1; i >= 0; i--)
        {
            if (activeGlitches[i] != null)
                activeGlitches[i].ForceResolve();
        }
        activeGlitches.Clear();
    }

    void ReceiveDonation()
    {
        float donationAmount = UnityEngine.Random.Range(Math.Max(attention - 50, 10), attention + 50);
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