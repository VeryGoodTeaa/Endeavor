using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Currencies")]
    public float attention;
    public float money;

    [Header("Stats")]
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

    public enum GameState { Play, UpgradeMode }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(PassiveLogicRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.UpgradeMode)
                UIManager.Instance.SetModeNormal();
            else
                SceneManager.LoadScene("MainMenu");
        }
    }

    public void HandleClick(float itemClickBonus, Vector3 clickPos)
    {
        float amount = currentClickPower + itemClickBonus;

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
}