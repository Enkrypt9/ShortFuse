using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHB : MonoBehaviour
{
    public GameObject anchor; //Enemy that the healthbar belongs to

    private GameObject health;

    private float lastFillPercent;
    private float fillPercent;
    public float disappearTimer;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        health = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (anchor.GetComponent<Enemy>().isBoss)
        {
            gameObject.SetActive(false);
        }

        fillPercent = anchor.GetComponent<Enemy>().health / anchor.GetComponent<Enemy>().originalhealth;
        health.GetComponent<RectTransform>().localScale = new Vector3(fillPercent, health.GetComponent<RectTransform>().localScale.y);

        if (fillPercent != lastFillPercent)
        {
            disappearTimer = 0;
        }
        else
        {
            disappearTimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;;

            if (disappearTimer >= 200)
            {
                GetComponent<Animator>().SetBool("Dying", true);
                health.SetActive(false);
            }
        }

        lastFillPercent = fillPercent;
    }

    public void ActivateHealth()
    {
        health.SetActive(true);
    }
    public void Kill()
    {
        Destroy(gameObject);
    }
}
