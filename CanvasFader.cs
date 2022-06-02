using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasFader : MonoBehaviour
{
    private float originalAlpha;
    private float alphaIncrement;
    private float currentAlpha;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            originalAlpha = GetComponent<Image>().color.a;
        }
        catch
        {
            try
            {
                originalAlpha = GetComponent<TextMeshProUGUI>().color.a;
            }
            catch
            {
                Debug.Log("ERROR WITH CANVAS FADE ALPHA OBJECT: " + gameObject.name);
            }
        }

        alphaIncrement = originalAlpha / 200;
        currentAlpha = originalAlpha;

    }
    public void BeginFadeOut()
    {
        InvokeRepeating("FadeOut", 0f, 0.01f);
    }
    public void BeginFadeIn()
    {
        InvokeRepeating("FadeIn", 0f, 0.01f);
    }
    void FadeOut()
    {
        currentAlpha -= alphaIncrement;

        if (currentAlpha > 0)
        {
            if (currentAlpha < alphaIncrement)
            {
                currentAlpha = 0;
            }
            try
            {
                Color someColor = GetComponent<Image>().color;
                someColor.a = currentAlpha;
                GetComponent<Image>().color = someColor;
            }
            catch
            {
                try
                {
                    Color someColor = GetComponent<TextMeshProUGUI>().color;
                    someColor.a = currentAlpha;
                    GetComponent<TextMeshProUGUI>().color = someColor;
                }
                catch
                {
                    Debug.Log("ERROR WITH CANVAS FADE OUT OBJECT: " + gameObject.name);
                }
            }
        }
        else
        {
            currentAlpha = 0f;

            CancelInvoke("FadeOut");
        }

    }
    void FadeIn()
    {
        currentAlpha += alphaIncrement;

        if (currentAlpha < originalAlpha)
        {
            if (currentAlpha > (originalAlpha - alphaIncrement))
            {
                currentAlpha = originalAlpha;
            }
            try
            {
                Color someColor = GetComponent<Image>().color;
                someColor.a = currentAlpha;
                GetComponent<Image>().color = someColor;
            }
            catch
            {
                try
                {
                    Color someColor = GetComponent<TextMeshProUGUI>().color;
                    someColor.a = currentAlpha;
                    GetComponent<TextMeshProUGUI>().color = someColor;
                }
                catch
                {
                    Debug.Log("ERROR WITH CANVAS FADE IN OBJECT: " + gameObject.name);
                }
            }
        }
        else
        {
            currentAlpha = originalAlpha;

            CancelInvoke("FadeIn");
        }

    }
}
