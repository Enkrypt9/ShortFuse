using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPScript : MonoBehaviour
{
    private SpriteRenderer sp;

    [SerializeField] private float originalFlashAmount;
    [SerializeField] private Color targetColor;
    private float flashMultiplier = 1f;
    private bool flashRed;
    private Color spColor;

    private GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        spColor = sp.color;

        explosion = transform.GetChild(0).gameObject;

        flashRed = true;

        InvokeRepeating("Flash", 0f, 0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Flash()
    {
        if (flashRed)
        {
            Color someColor = sp.color;

            if (someColor.g > targetColor.g + (originalFlashAmount * flashMultiplier))
            {
                someColor.g -= originalFlashAmount * flashMultiplier;
            }
            else
            {
                someColor.g = targetColor.g;
            }
            if (someColor.b > targetColor.b + (originalFlashAmount * flashMultiplier))
            {
                someColor.b -= originalFlashAmount * flashMultiplier;
            }
            else
            {
                someColor.b = targetColor.b;
                flashRed = false;
            }

            sp.color = someColor;
        }
        else
        {
            Color someColor = sp.color;

            if (someColor.g < spColor.g - (originalFlashAmount * flashMultiplier))
            {
                someColor.g += originalFlashAmount * flashMultiplier;
            }
            else
            {
                someColor.g = spColor.g;
            }
            if (someColor.b < spColor.b - (originalFlashAmount * flashMultiplier))
            {
                someColor.b += originalFlashAmount * flashMultiplier;
            }
            else
            {
                someColor.b = spColor.b;
                flashRed = true;
            }

            sp.color = someColor;
        }

        if (sp.color == spColor)
        {
            if (flashMultiplier < 2.2f)
            {
                flashMultiplier += 0.2f;
            }
            else
            {
                explosion.GetComponent<ParticleSystem>().Play();
                GetComponent<SpriteRenderer>().enabled = false;
                Invoke("DestroyEMP", 1.5f);
                CancelInvoke("Flash");
            }
        }
    }

    void DestroyEMP()
    {
        Destroy(gameObject);
    }
}
