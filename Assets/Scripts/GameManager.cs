using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int attentionInc = 1;
    private int attentionBaseDec = 1;
    private int attentionDecInterval = 5;
    private int attentionCount = 0;

    // private int moneyCount = 0;

    public bool IsUpgradeMode { get; set; }  = false;

    public TextMeshProUGUI AttentionCountText;
    public TextMeshProUGUI MoneyCountText;

    public GameObject DonutPrefab;
    public GameObject PopupPrefab;
    public Transform Parent;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnClick()
    {
        int attentionGained = attentionInc;

        attentionCount += attentionGained;
        AttentionCountText.text = attentionCount.ToString();

        // ShowPopup(attentionGained);
    }

    public void Update()
    {
        // random -> spawn donuts -> inc money
        // if upgrade mode -> stop


    }

    public void UpgradeDesk()
    {
        IsUpgradeMode = true;
    }

    public void UpgradeRoom()
    {
        IsUpgradeMode = true;
        UIManager.Instance.OpenPanel(UIManager.Instance.RoomUpgradesPanel);
    }

    private void ShowPopup(int amount)
    {
        Vector3 mousePos = Input.mousePosition;

        float randomX = Random.Range(-20f, 20f);
        float randomY = Random.Range(-20f, 20f);
        Vector3 spawnPos = mousePos + new Vector3(randomX, randomY, 0);

        GameObject popup = Instantiate(PopupPrefab, spawnPos, Quaternion.identity, Parent);

        Popup script = popup.GetComponent<Popup>();
        if (script != null)
            script.Setup(amount);
    }
}