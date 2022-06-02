using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EasterEggs : MonoBehaviour
{
    public GameObject handler;
    private GameObject settings;

    public int number;

    public bool isText;

    private void Start()
    {
        settings = handler.GetComponent<MenuHandler>().settings;
    }

    // Update is called once per frame
    void Update()
    {
        if (settings == null)
        {
            settings = handler.GetComponent<MenuHandler>().settings;
        }
        else
        {
            if (!isText)
            {
                if (settings.GetComponent<Settings>().easterEggs[number - 1] == true)
                {
                    GetComponent<Image>().color = Color.white;
                }
                else
                {
                    GetComponent<Image>().color = Color.black;
                }
            }
            else
            {
                if (settings.GetComponent<Settings>().easterEggs[number - 1] == true)
                {
                    GetComponent<TextMeshProUGUI>().text = handler.GetComponent<MenuHandler>().easterEggTitles[number - 1];
                }
                else
                {
                    GetComponent<TextMeshProUGUI>().text = "???";
                }
            }
        }
    }
}
