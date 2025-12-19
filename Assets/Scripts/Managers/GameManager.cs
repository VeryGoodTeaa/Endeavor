using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Resources")]
    public float attention;
    public float money;

    [Header("Stats")]
    public float clickPower = 1f;
    public float passiveAttentionRate = -0.5f; // Внимание падает по умолчанию
    public float baseDonationChance = 0.05f; // 5% шанс в секунду

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(PassiveLoop());
    }

    private void Update()
    {
        attention += passiveAttentionRate * Time.deltaTime;

        if (attention < 0) attention = 0;

        UIManager.Instance.UpdateResourceUI(attention, money);
    }

    public void AddAttention(float amount)
    {
        attention += amount;
    }

    IEnumerator PassiveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            float currentChance = baseDonationChance + (attention * 0.001f);

            if (Random.value < currentChance)
                GenerateDonation();
        }
    }

    void GenerateDonation()
    {
        int donationAmount = Mathf.RoundToInt(Random.Range(10, 50) * (1 + attention * 0.01f));
        money += donationAmount;
        UIManager.Instance.AddDonationLog(donationAmount);
    }

    public bool TrySpendMoney(float amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        return false;
    }

    public void ApplyUpgradeBonuses(UpgradeLevelData data)
    {
        clickPower += data.clickPowerBonus;
        passiveAttentionRate += data.passiveAttentionBonus;
        // events logic
    }
}