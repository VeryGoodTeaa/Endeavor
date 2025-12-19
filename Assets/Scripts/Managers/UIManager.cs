using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public TMP_Text attentionText;
    public TMP_Text moneyText;
    public TMP_Dropdown modeDropdown;
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    [Header("Donation List")]
    public Transform donationListContainer;
    public GameObject donationItemPrefab;

    [Header("Click Popup")]
    public GameObject clickPopupPrefab;
    public Canvas worldCanvas;

    private void Awake() => Instance = this;

    public void UpdateResourceUI(float attention, float money)
    {
        attentionText.text = $"{attention:F0}";
        moneyText.text = $"${money:F0}";
    }

    public void OnDropdownChanged(int index)
    {
        FindObjectOfType<InteractionManager>().SetMode(index);
    }

    public void ResetDropdown()
    {
        modeDropdown.value = 0;
    }

    public void ShowTooltip(string name, float cost, float bonus, Vector2 screenPos)
    {
        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = screenPos + new Vector2(20, 20);
        tooltipText.text = $"{name}\nCost: {cost}$\nBonus: +{bonus}";
    }

    public void ShowTooltip(string name, string customMsg, Vector2 screenPos)
    {
        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = screenPos;
        tooltipText.text = $"{name}\n{customMsg}";
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    public void AddDonationLog(int amount)
    {
        GameObject item = Instantiate(donationItemPrefab, donationListContainer);
        item.GetComponentInChildren<TMP_Text>().text = $"+{amount}$";

        if (donationListContainer.childCount > 4)
            Destroy(donationListContainer.GetChild(0).gameObject);

        Destroy(item, 5.0f);
    }

    public void SpawnClickPopup(Vector2 worldPos, float amount)
    {
        Vector2 randomOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        GameObject popup = Instantiate(clickPopupPrefab, worldPos + randomOffset, Quaternion.identity);

        popup.GetComponentInChildren<TMP_Text>().text = $"+{amount}";

        Destroy(popup, 1.0f);
    }
}