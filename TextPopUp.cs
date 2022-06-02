using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopUp : MonoBehaviour
{

    public int damage;
    public int totalDamage;
    public string text;
    public GameObject target;
    public Vector3 targetScale;
    private TextMeshProUGUI tmp;
    private float alpha;

    private bool dying;
    public float deathTime;
    public bool crit;
    public Color critColor;

    private GameObject dmgNumbers;

    private float bounceMult;
    private bool bounceUp;
    private bool bouncing;

    public int stacks;

    private float originalFontSize;
    private Vector3 originalPos;
    private Vector3 originalTargetPos;

    private float floatHeight;

    // Start is called before the first frame update
    void Start()
    {
        originalTargetPos = transform.position;

        stacks = 1;
        bounceMult = 1f;
        bounceUp = true;
        bouncing = false;

        floatHeight = 0f;

        totalDamage = damage;

        alpha = 1f;
        dying = false;

        //float offsetX = Random.Range(-0.5f, 0.5f);
        float offsetX = 0f;
        float offsetY = Random.Range(0.6f, 0.8f);

        //offsetY = offsetY * target.transform.localScale.x;

        transform.position = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY);

        originalPos = transform.position;

        dmgNumbers = GameObject.Find("DamageNumbers");
        
        foreach (Transform child in dmgNumbers.transform)
        {
            if (child.GetComponent<TextPopUp>().target == target)
            {
                if (crit)
                {
                    child.GetComponent<TextPopUp>().StartBounce();
                }

                child.GetComponent<TextPopUp>().totalDamage += damage;
                child.GetComponent<TextPopUp>().text = totalDamage.ToString();
                child.GetComponent<TextPopUp>().stacks += 1;

                /* //Colour based on stacks
                if (child.GetComponent<TextMeshProUGUI>().color.g > 0.05f)
                {
                    float g = child.GetComponent<TextMeshProUGUI>().color.g - 0.05f;
                    Color x = new Color(child.GetComponent<TextMeshProUGUI>().color.r, g, child.GetComponent<TextMeshProUGUI>().color.b);
                    child.GetComponent<TextMeshProUGUI>().color = x;
                }
                */

                child.GetComponent<TextPopUp>().Reset();

                Destroy(gameObject);
                break;
            }
        }
        
        transform.SetParent(dmgNumbers.transform);
        tmp = GetComponent<TextMeshProUGUI>();
        Invoke("Death", deathTime);

        originalFontSize = tmp.fontSize;

        if (crit)
        {
            //tmp.color = critColor;
            StartBounce();
        }
    }

    // Update is called once per frame
    void Update()
    {

        Bounce();
        transform.localScale = new Vector3(1f, 1f, 1f) * bounceMult;

        Color newColor = tmp.color;
        newColor.g = 0.75f - (0.005f * totalDamage);
        tmp.color = newColor;

        if (target != null)
        {
            Vector3 offset = target.transform.position - originalTargetPos;
            transform.position = new Vector3(originalPos.x + offset.x, originalPos.y + offset.y + floatHeight);

            targetScale = target.transform.localScale;
        }

        if (stacks < 10)
        {
            tmp.fontSize = originalFontSize + ((stacks - 1) * 0.04f);
        }

        tmp.text = totalDamage.ToString();


        if (floatHeight < (0.4f * targetScale.y))
        {
            floatHeight += 0.01f * targetScale.y * 60f / (1f / Time.deltaTime); //Change first number for rate;
        }

        if (dying)
        {
            if (alpha > 0.04f)
            {
                alpha -= 0.04f * 60f / (1f / Time.deltaTime); //Change first number for rate;

                Color x = tmp.color;
                x.a = alpha;
                tmp.color = x;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void Death()
    {
        dying = true;
    }
    public void Reset()
    {
        dying = false;

        alpha = 1f;

        Color x = tmp.color;
        x.a = alpha;
        tmp.color = x;

        CancelInvoke("Death");
        Invoke("Death", deathTime);
    }
    void StartBounce()
    {
        bouncing = true;
    }
    void Bounce()
    {
        if (bouncing)
        {
            if (bounceUp)
            {
                if (bounceMult < 1.3f)
                {
                    bounceMult += 0.04f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }
                else
                {
                    bounceUp = false;
                }
            }
            else
            {
                if (bounceMult > 1f)
                {
                    bounceMult -= 0.04f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }
                else
                {
                    bouncing = false;
                    bounceUp = true;
                }
            }
        }
    }
}
