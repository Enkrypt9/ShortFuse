using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private GameHandler handler;
    public bool locked;
    public Vector2 pos;
    public GameObject nextTeleporter;
    public bool entry;
    public int level;

    private bool turnedOff;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();

        pos = nextTeleporter.transform.position;

        animator = GetComponent<Animator>();
        if (entry)
        {
            locked = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (handler.currentLevel == level)
        {
            if (!entry)
            {
                if (handler.killed < handler.enemyNo[handler.currentLevel - 1])
                {
                    GetComponent<SpriteRenderer>().sprite = handler.Telepads[1];
                    locked = true;
                    animator.SetBool("Unlocked", false);
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = handler.Telepads[0];
                    locked = false;
                    animator.SetBool("Unlocked", true);
                }
            }
            else
            {
                locked = true;
                if (!turnedOff)
                {
                    turnedOff = true;

                    GetComponent<SpriteRenderer>().sprite = handler.Telepads[0];
                    animator.SetBool("Unlocked", true);

                    Invoke("TurnOff", 6f);
                }
            }
        }
        else
        {
            if (!locked)
            {
                GetComponent<SpriteRenderer>().sprite = handler.Telepads[1];
                locked = true;
                animator.SetBool("Unlocked", false);
            }
        }
    }

    void TurnOff()
    {
        handler.tpParticles.GetComponent<SpriteRenderer>().enabled = false;

        GetComponent<SpriteRenderer>().sprite = handler.Telepads[1];
        locked = true;
        animator.SetBool("Unlocked", false);
    }
}
