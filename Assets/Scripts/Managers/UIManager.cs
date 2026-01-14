using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private Queue<DonationItem> donationItemsQueue = new Queue<DonationItem>();

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
    public GameObject popupMonitorPrefap;
    public Transform popupContainer;

    [Header("WebCam")]
    public GameObject RoomImage;
    public GameObject CouchImage;
    public GameObject FlowerImage;

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
        tooltipObj.transform.position = Input.mousePosition + new Vector3(100, 20, 0);
        tooltipText.text = $"Cost: {cost}$\n+Passive: {passive}\n+Click: {click}";
    }

    public void ShowMaxLevelTooltip()
    {
        tooltipObj.SetActive(true);
        tooltipObj.transform.position = Input.mousePosition + new Vector3(170, 20, 0);
        tooltipText.text = "MAX LEVEL";
    }

    public void HideTooltip()
    {
        tooltipObj.SetActive(false);
    }

    public void ShowClickPopup(string text, Vector3 pos, bool isMonitor = false, int size = 30)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);

        GameObject popup;
        if (isMonitor)
        {
            popup = Instantiate(popupMonitorPrefap, pos + randomOffset, Quaternion.identity, popupContainer);
        }
        else 
        { 
            popup = Instantiate(popupPrefab, pos + randomOffset, Quaternion.identity, popupContainer);
        }

        popup.GetComponentInChildren<TMP_Text>().text = text;
        popup.GetComponentInChildren<TMP_Text>().fontSize = size;
        Destroy(popup, 1.5f);
    }

    public void AddDonationLog(float amount)
    {
        GameObject itemObj = Instantiate(donationItemPrefab, donationListContainer);
        itemObj.GetComponentInChildren<TMP_Text>().text = $"{amount:F0}";

        DonationItem donationScript = itemObj.GetComponent<DonationItem>();

        donationItemsQueue.Enqueue(donationScript);

        if (donationItemsQueue.Count > 4)
        {
            DonationItem oldItem = donationItemsQueue.Dequeue();

            if (oldItem != null)
                oldItem.ForceFadeOutAndDestroy();
        }
    }
}