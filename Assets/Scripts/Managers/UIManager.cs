using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Roots")]
    public GameObject mainScreenRoot;


    [Header("Cameras & Textures")]
    public Camera extraCamera;      
    public RenderTexture miniViewTexture; 
    public Canvas mainCanvas;      

    [Header("HUD")]
    public TMP_Text attentionText;
    public TMP_Text moneyText;
    public TMP_Dropdown modeDropdown;
    public GameObject tooltipObj;
    public TMP_Text tooltipText;

    [Header("Donation Log")]
    public Transform donationListContainer;
    public GameObject donationItemPrefab;
    private Queue<DonationItem> donationItemsQueue = new Queue<DonationItem>();

    [Header("Popups")]
    public GameObject popupPrefab;
    public Transform popupContainer;

    private void Awake()
    {
        Instance = this;
        modeDropdown.onValueChanged.AddListener(OnModeChanged);
    }

    private void Start()
    {
        SetModeNormal();
    }

    public void OnModeChanged(int index)
    {
        switch (index)
        {
            case 0:
                SetModeNormal();
                break;
            case 1:
                SetModeMainUpgrade();
                break;
            case 2:
                SetModeExtraUpgrade();
                break;
        }
    }

    public void SetModeNormal()
    {
        GameManager.Instance.currentState = GameManager.GameState.Play;
        modeDropdown.value = 0;

        mainScreenRoot.SetActive(true);
        mainCanvas.enabled = true;
        extraCamera.targetTexture = miniViewTexture;

        HideTooltip();
    }

    void SetModeMainUpgrade()
    {
        GameManager.Instance.currentState = GameManager.GameState.UpgradeMode;

        mainScreenRoot.SetActive(true);
        mainCanvas.enabled = true;
        extraCamera.targetTexture = miniViewTexture;
    }

    void SetModeExtraUpgrade()
    {
        GameManager.Instance.currentState = GameManager.GameState.UpgradeMode;

        mainCanvas.enabled = false;
        // Примечание: Dropdown исчезнет вместе с канвасом! 
        // Чтобы выйти назад, нам нужно либо вынести Dropdown в отдельный Canvas, 
        // который всегда поверх всего, либо обрабатывать нажатие ESC.
        extraCamera.targetTexture = null;
    }


    public void UpdateCurrencyUI()
    {
        attentionText.text = $"{GameManager.Instance.attention:F0}";
        moneyText.text = $"{GameManager.Instance.money:F0}";
    }

    public void ShowTooltip(float cost, float passive, float click)
    {
        tooltipObj.SetActive(true);
        tooltipObj.transform.position = Input.mousePosition + new Vector3(20, 20, 0);
        tooltipText.text = $"Cost: {cost}$\n+Passive: {passive}\n+Click: {click}";
    }

    public void ShowMaxLevelTooltip()
    {
        tooltipObj.SetActive(true);
        tooltipObj.transform.position = Input.mousePosition + new Vector3(20, 20, 0);
        tooltipText.text = "MAX LEVEL";
    }

    public void HideTooltip()
    {
        tooltipObj.SetActive(false);
    }

    public void ShowClickPopup(float amount, Vector3 pos)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);

        GameObject popup = Instantiate(popupPrefab, pos + randomOffset, Quaternion.identity, popupContainer);
        popup.GetComponentInChildren<TMP_Text>().text = $"+{amount:F0}";
    }

    public void AddDonationLog(float amount)
    {
        GameObject itemObj = Instantiate(donationItemPrefab, donationListContainer);
        itemObj.GetComponentInChildren<TMP_Text>().text = $"+{amount:F0}";
        DonationItem donationScript = itemObj.GetComponent<DonationItem>();
        donationItemsQueue.Enqueue(donationScript);

        if (donationItemsQueue.Count > 4)
        {
            DonationItem oldItem = donationItemsQueue.Dequeue();
            if (oldItem != null) oldItem.ForceFadeOutAndDestroy();
        }
    }
}