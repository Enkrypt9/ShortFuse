using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{
    public float killTime;

    public bool fade;
    public float startFadeTime;
    public float fadeSpeed;

    private float fadeAlpha;
    private bool fadeStarted;

    private SpriteRenderer spRend;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, killTime);

        if (fade)
        {
            spRend = GetComponent<SpriteRenderer>();

            fadeAlpha = spRend.color.a;
            fadeStarted = false;

            Invoke("StartFade", startFadeTime);
        }
    }

    void Update()
    {
        if (fadeStarted)
        {
            if (fadeAlpha > fadeSpeed)
            {
                fadeAlpha -= fadeSpeed * 60f / (1f / Time.deltaTime); //Change first number for rate;;
            }
            else
            {
                fadeAlpha = 0f;
            }

            Color x = spRend.color;
            x.a = fadeAlpha;
            spRend.color = x;
        }
    }

    void StartFade()
    {
        fadeStarted = true;
    }

}
