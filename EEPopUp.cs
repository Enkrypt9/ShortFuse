using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EEPopUp : MonoBehaviour
{
    public GameObject popup;
    private GameObject icon;
    private GameObject nameText;
    private RectTransform popupTransform;

    public Sprite[] icons;
    public string[] names;
    public Vector2[] iconSizes;

    public float speed;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        icon = popup.transform.GetChild(0).gameObject;
        nameText = popup.transform.GetChild(2).gameObject;
        popupTransform = popup.GetComponent<RectTransform>();
    }

    public void BeginPopUp(int number)
    {
        icon.GetComponent<Image>().sprite = icons[number];
        nameText.GetComponent<TextMeshProUGUI>().text = names[number];

        icon.GetComponent<RectTransform>().sizeDelta = iconSizes[number];

        InvokeRepeating("FlyIn", 0f, speed);
    }

    void FlyIn()
    {
        if (popupTransform.anchoredPosition.x > 720)
        {
            popupTransform.anchoredPosition = new Vector2(popupTransform.anchoredPosition.x - 5, popupTransform.anchoredPosition.y);
        }

        else
        {
            popupTransform.anchoredPosition = new Vector2(720, popupTransform.anchoredPosition.y);
            CancelInvoke("FlyIn");
            InvokeRepeating("FlyOut", duration, speed);
        }
    }
    void FlyOut()
    {
        if (popupTransform.anchoredPosition.x < 1250)
        {
            popupTransform.anchoredPosition = new Vector2(popupTransform.anchoredPosition.x + 5, popupTransform.anchoredPosition.y);
        }

        else
        {
            popupTransform.anchoredPosition = new Vector2(1250, popupTransform.anchoredPosition.y);
            CancelInvoke("FlyOut");
        }
    }
}
