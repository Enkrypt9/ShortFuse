using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    private MenuHandler handler;
    private bool unlocked;
    public int level;

    // Start is called before the first frame update
    void Start()
    {
        handler = GameObject.Find("Handler").GetComponent<MenuHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (handler.settings.GetComponent<Settings>().maxLevel >= level)
        {
            unlocked = true;
            //GetComponent<Image>().color = Color.green;
            GetComponent<Button>().interactable = true;
        }
        else
        {
            unlocked = false;
            //GetComponent<Image>().color = Color.red;
            GetComponent<Button>().interactable = false;
        }
    }
    public void OnClicked()
    {
        handler.settings.GetComponent<Settings>().currentLevel = level;
        handler.settings.GetComponent<Settings>().destination = handler.settings.GetComponent<Settings>().levelPos[handler.settings.GetComponent<Settings>().currentLevel - 1];
    }
}
