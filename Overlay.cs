using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    public float pulseRate;

    private Image img;
    private bool alphaUp;
    private float originalAlpha;
    private float currentAlpha;
    private bool ending; //Whether this is the last pulse before it returns to original alpha
    private bool pulsing;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        originalAlpha = img.color.a;
        currentAlpha = originalAlpha;

        alphaUp = true;
    }

    public void StartPulse()
    {
        InvokeRepeating("Pulse", 0f, pulseRate);
    }

    public void StopPulse()
    {
        if (pulsing)
        {
            ending = true;
            alphaUp = false;
        }
    }

    public void Pulse()
    {
        pulsing = true;

        if (alphaUp)
        {
            if (img.color.a >= 1f)
            {
                alphaUp = false;
            }
            else
            {
                currentAlpha += 0.01f;
            }
        }
        else
        {
            if (img.color.a <= originalAlpha)
            {
                if (!ending)
                {
                    alphaUp = true;
                }
                else
                {
                    currentAlpha = originalAlpha;
                    alphaUp = true;
                    ending = false;
                    pulsing = false;

                    CancelInvoke("Pulse");
                }
            }
            else
            {
                currentAlpha -= 0.01f;
            }
        }

        Color someColor = img.color;
        someColor.a = currentAlpha;
        img.color = someColor;
    }
}
