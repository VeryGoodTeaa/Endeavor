using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Roots")]
    public GameObject mainScreenRoot;
    public GameObject extraScreenRoot;

    [Header("HUD")]
    public TMP_Text attentionText;
    public TMP_Text moneyText;
    public TMP_Dropdown modeDropdown;
    public GameObject tooltipObj;
    public TMP_Text tooltipText;

    [Header("Donation Log")]
    public Transform donationListContainer;
    public GameObject donationItemPrefab;
    private Queue<GameObject> donationItems = new Queue<GameObject>();

    [Header("Popups")]
    public GameObject popupPrefab;
    public Transform popupContainer;

    private void Awake()
    {
        Instance = this;
        modeDropdown.onValueChanged.AddListener(OnModeChanged);
    }

    public void OnModeChanged(int index)
    {
        switch (index)
        {
            case 0:
                SetModeNormal();
                break;
            case 1:
                GameManager.Instance.currentState = GameManager.GameState.UpgradeMode;
                mainScreenRoot.SetActive(true);
                extraScreenRoot.SetActive(false);
                break;
            case 2:
                GameManager.Instance.currentState = GameManager.GameState.UpgradeMode;
                mainScreenRoot.SetActive(false);
                extraScreenRoot.SetActive(true);
                break;
        }
    }

    public void SetModeNormal()
    {
        GameManager.Instance.currentState = GameManager.GameState.Play;
        modeDropdown.value = 0;
        mainScreenRoot.SetActive(true);
        extraScreenRoot.SetActive(false);
        HideTooltip();
    }

    public void UpdateCurrencyUI()
    {
        attentionText.text = $"{GameManager.Instance.attention:F0}";
        moneyText.text = $"{GameManager.Instance.money:F0}";
    }

    public void ShowTooltip(float cost, float passive, float click)
    {
        tooltipObj.SetActive(true);
        tooltipObj.transform.position = Input.mousePosition + new Vector3(100, 50, 0);
        tooltipText.text = $"Цена: {cost}$\n+Пассивный бонус: {passive}\n+Сила клика: {click}";
    }

    public void ShowMaxLevelTooltip()
    {
        tooltipObj.SetActive(true);
        tooltipObj.transform.position = Input.mousePosition + new Vector3(20, 20, 0);
        tooltipText.text = "MAX LEVEL";
    }

    public void HideTooltip() => tooltipObj.SetActive(false);

    public void ShowClickPopup(float amount, Vector3 pos)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);
        GameObject popup = Instantiate(popupPrefab, pos + randomOffset, Quaternion.identity, popupContainer);
        popup.GetComponentInChildren<TMP_Text>().text = $"+{amount:F0}";
        Destroy(popup, 0.8f); // ToDo - objects pool
    }

    public void AddDonationLog(float amount)
    {
        GameObject item = Instantiate(donationItemPrefab, donationListContainer);
        item.GetComponentInChildren<TMP_Text>().text = $"+{amount}$";

        donationItems.Enqueue(item);

        if (donationItems.Count > 4)
        {
            GameObject oldItem = donationItems.Dequeue();
            Destroy(oldItem);
        }

        StartCoroutine(RemoveDonationOverTime(item));
    }

    System.Collections.IEnumerator RemoveDonationOverTime(GameObject item)
    {
        yield return new WaitForSeconds(5f);
        if (item != null)
            Destroy(item);
    }
}
