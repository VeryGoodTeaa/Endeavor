using TMPro;
using System.Collections;
using UnityEngine;

public class ClickerScript : MonoBehaviour
{
    public int attention;
    public TMPro.TextMeshProUGUI text;

    public void Click()
    {
        attention += 1;  
        text.text = attention.ToString();
    }
}
