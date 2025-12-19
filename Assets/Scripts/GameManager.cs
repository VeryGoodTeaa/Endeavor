using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int attentionCount = 0;
    public TextMeshProUGUI attentionCountText;
    public GameObject popupPrefab;
    public Transform popupParent;

    public void OnClick()
    {
        int attentionGained = 2;

        attentionCount += attentionGained;
        attentionCountText.text = attentionCount.ToString();


        ShowPopup(attentionGained);
    }

    private void ShowPopup(int amount)
    {
        Vector3 mousePos = Input.mousePosition;

        float randomX = Random.Range(-20f, 20f);
        float randomY = Random.Range(-20f, 20f);
        Vector3 spawnPos = mousePos + new Vector3(randomX, randomY, 0);

        GameObject popup = Instantiate(popupPrefab, spawnPos, Quaternion.identity, popupParent);

        Popup script = popup.GetComponent<Popup>();
        if (script != null)
            script.Setup(amount);
    }
}