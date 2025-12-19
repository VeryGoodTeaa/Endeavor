using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClickerScript : MonoBehaviour
{
    public GameObject popupPrefab;
    public Canvas canvas; // Ссылка на Canvas, где будет отображаться всплывающее изображение
    public int attention;
    public TMPro.TextMeshProUGUI text;

    public void Click()
    {
        attention += 1;  
        text.text = attention.ToString();
    }


}


//public class ClickerButtonHandler : MonoBehaviour
//{


//    public void OnClick()
//    {
//        // Получаем позицию клика
//        Vector2 clickPosition = Input.mousePosition;

//        // Создаем экземпляр префаба
//        GameObject popupInstance = Instantiate(popupPrefab, canvas.transform);

//        // Устанавливаем позицию всплывающего изображения
//        popupInstance.transform.position = clickPosition;

//        // Настраиваем текст и значок валюты
//        Text currencyText = popupInstance.GetComponentInChildren<Text>();
//        if (currencyText != null)
//        {
//            currencyText.text = "+10"; // Укажите количество валюты
//        }

//        Image currencyIcon = popupInstance.GetComponentInChildren<Image>();
//        if (currencyIcon != null)
//        {
//            // Установите значок валюты, если он есть
//            // Например, currencyIcon.sprite = yourCurrencySprite;
//        }

//        // Уничтожаем всплывающее изображение через некоторое время
//        Destroy(popupInstance, 1.5f); // Уничтожить через 1.5 секунды
//    }
//}
