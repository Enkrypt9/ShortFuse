using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : MonoBehaviour
{
    public int type; //1 = Pulsing
    private SpriteRenderer spRend;
    private Color originalColor;
    public Color fadeTargetColor;

    public float fadeSpeed;

    private bool fadeIn;

    private float rainbowHue = 0f;

    // Start is called before the first frame update
    void Start()
    {
        fadeIn = false;
        spRend = GetComponent<SpriteRenderer>();
        originalColor = spRend.color;

        if (type == 1)
        {
            InvokeRepeating("Fade", 0f, fadeSpeed);
        }
        else if (type == 2) //Rainbow
        {
            InvokeRepeating("Rainbow", 0f, fadeSpeed);
        }
    }

    void Fade()
    {
        if (fadeIn)
        {
            Color x = spRend.color;

            if (x.r < originalColor.r - 0.01f)
            {
                x.r += 0.01f;
            }
            else
            {
                x.r = originalColor.r;
            }
            if (x.g < originalColor.g - 0.01f)
            {
                x.g += 0.01f;
            }
            else
            {
                x.g = originalColor.g;
            }
            if (x.b < originalColor.b - 0.01f)
            {
                x.b += 0.01f;
            }
            else
            {
                x.b = originalColor.b;
            }

            spRend.color = x;

            if (spRend.color == originalColor)
            {
                fadeIn = false;
            }
        }
        else
        {
            Color x = spRend.color;

            if (x.r > fadeTargetColor.r + 0.01f)
            {
                x.r -= 0.01f;
            }
            else
            {
                x.r = fadeTargetColor.r;
            }
            if (x.g > fadeTargetColor.g + 0.01f)
            {
                x.g -= 0.01f;
            }
            else
            {
                x.g = fadeTargetColor.g;
            }
            if (x.b > fadeTargetColor.b + 0.01f)
            {
                x.b -= 0.01f;
            }
            else
            {
                x.b = fadeTargetColor.b;
            }

            spRend.color = x;

            if (spRend.color == fadeTargetColor)
            {
                fadeIn = true;
            }
        }
    }
    void Rainbow()
    {
        Color x = spRend.color;

        rainbowHue += 0.004f;

        if (rainbowHue > 1f)
        {
            rainbowHue = 0f;
        }

        x = Color.HSVToRGB(rainbowHue, 1f, 1f);

        spRend.color = x;
    }
}
