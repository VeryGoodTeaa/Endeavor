using TMPro;
using System.Collections;
using UnityEngine;

public class ClickerScript : MonoBehaviour
{
    public int money;
    public TMPro.TextMeshProUGUI text;

    public void Click()
    {
        money += 1;  
        text.text = money.ToString();
    }
}
