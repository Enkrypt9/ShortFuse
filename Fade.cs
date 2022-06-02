using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fade : MonoBehaviour
{
    private GameObject handlerobj;
    private GameHandler handler;
    public float alpha;
    private float duration;
    private float stay_time;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            handlerobj = GameObject.Find("GameHandler");
            handler = handlerobj.GetComponent<GameHandler>();
        }
        catch
        {
            handlerobj = GameObject.Find("Handler");
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            Color x = GetComponent<SpriteRenderer>().color;
            x.a = alpha;
            GetComponent<SpriteRenderer>().color = x;
        }
        catch
        {
            Color x = GetComponent<Image>().color;
            x.a = alpha;
            GetComponent<Image>().color = x;
        }

        bool fadeCheck = false;
        try //To make sure not to double fade when cinematic bars appear
        {
            if (!gameObject.transform.parent.GetComponent<FollowCamera>().cutsceneActive)
            {
                fadeCheck = true;
            }
        }
        catch //For start menu
        {
            fadeCheck = true;
        }

        
        if (fadeCheck)
        {
            foreach (Transform text in GameObject.Find("Canvas").transform) //Make this long if statement an array of names that shouldn't fade
            {
                if (text.gameObject.name != "FadeCreds" && text.gameObject.name != "StartFader" && handlerobj.gameObject.name != "Handler" && text.gameObject.name != "ShopMenu" && text.gameObject.name != "ScreenCrack(Clone)")
                {
                    if (text.gameObject.name != "PauseMenu" || handler.FadePauseMenu)
                    {
                        try
                        {
                            Color y = text.GetComponent<TextMeshProUGUI>().color;
                            y.a = 1f - alpha;
                            text.GetComponent<TextMeshProUGUI>().color = y;
                        }
                        catch
                        {
                            try
                            {
                                Color z = text.GetComponent<Image>().color;
                                z.a = 1f - alpha;
                                text.GetComponent<Image>().color = z;
                            }
                            catch
                            {
                                ;
                            }
                        }
                        try
                        {
                            foreach (Transform child in text)
                            {
                                if (child.gameObject.name != "UltReady" && child.gameObject.name != "Overheated" && child.gameObject.name != "VisorLines" && child.gameObject.name != "Pause" && child.gameObject.name != "Overlay" && child.gameObject.name != "EEPopup" && child.gameObject.name != "BossHealthBar")
                                {
                                    try
                                    {
                                        Color y = child.GetComponent<TextMeshProUGUI>().color;
                                        y.a = 1f - alpha;
                                        child.GetComponent<TextMeshProUGUI>().color = y;
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            Color z = child.GetComponent<Image>().color;
                                            z.a = 1f - alpha;
                                            child.GetComponent<Image>().color = z;
                                        }
                                        catch
                                        {
                                            ;
                                        }
                                    }
                                    try
                                    {
                                        foreach (Transform grandchild in child)
                                        {
                                            try
                                            {
                                                Color y = grandchild.GetComponent<TextMeshProUGUI>().color;
                                                y.a = 1f - alpha;
                                                grandchild.GetComponent<TextMeshProUGUI>().color = y;
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    Color z = grandchild.GetComponent<Image>().color;
                                                    z.a = 1f - alpha;
                                                    grandchild.GetComponent<Image>().color = z;
                                                }
                                                catch
                                                {
                                                    ;
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        ;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            ;
                        }
                    }
                }
            }
        }
    }
    public void Fader(float durationinput, float stay_timeinput)
    {
        duration = durationinput;
        stay_time = stay_timeinput;
        InvokeRepeating("Fade_In", 0f, 0.01f);
        Invoke("Stop_Fade_In", duration);
    }
    public void FaderOut(float durationinput, float stay_timeinput)
    {
        alpha = 1f;
        duration = durationinput;
        stay_time = stay_timeinput;
        Invoke("StartFadeOut", stay_time);
    }
    public void FaderIn(float durationinput, float stay_timeinput)
    {
        alpha = 0f;
        duration = durationinput;
        stay_time = stay_timeinput;
        Invoke("StartFadeIn", stay_time);
    }

    void StartFadeOut()
    {
        InvokeRepeating("Fade_Out", 0f, 0.01f);
    }
    void StartFadeIn()
    {
        InvokeRepeating("Fade_In", 0f, 0.01f);
    }
    void Fade_In()
    {
        float x = duration / 0.01f;
        float y = 1f / x;

        alpha += y * 60f / (1f / Time.deltaTime); //Change first number for rate;

        if (alpha > 1f)
        {
            alpha = 1f;
        }
    }
    void Stop_Fade_In()
    {
        alpha = 1f;
        CancelInvoke("Fade_In");
        InvokeRepeating("Fade_Out", duration + stay_time, 0.01f);
    }
    void Fade_Out()
    {
        try
        {
            if (handler.player.GetComponent<SpriteRenderer>().color.a != 1f)
            {
                Color a = handler.player.GetComponent<SpriteRenderer>().color;
                a.a = 1f;
                handler.player.GetComponent<SpriteRenderer>().color = a;

                foreach (Transform child in handler.player.transform)
                {
                    if (child.gameObject.name != "Shadow")
                    {
                        Color b = child.GetComponent<SpriteRenderer>().color;
                        b.a = 1f;
                        child.GetComponent<SpriteRenderer>().color = b;

                        if (child.gameObject.name == "Hair")
                        {
                            Color c = child.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                            c.a = 1f;
                            child.transform.GetChild(0).GetComponent<SpriteRenderer>().color = c;
                        }
                    }
                }
            }
        }
        catch
        {
            ;
        }
        float x = duration / 0.01f;
        float y = 1f / x;

        alpha -= y * 60f / (1f / Time.deltaTime); //Change first number for rate;

        if (alpha < 0f)
        {
            alpha = 0f;
            CancelInvoke("Fade_Out");
        }
    }
}
