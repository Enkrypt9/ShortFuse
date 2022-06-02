using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public float health;
    private GameHandler handler;
    private Animator animator;
    private bool isDestroyed;
    private float timer;
    private bool fading;
    private float alpha;

    private float originalhealth;

    public bool isGolden;

    // Start is called before the first frame update
    void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        animator = GetComponent<Animator>();
        isDestroyed = false;
        timer = 0f;
        fading = false;
        alpha = 1f;

        originalhealth = health;

        if (isGolden)
        {
            animator.SetBool("Golden", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDestroyed)
        {
            if (health <= 0)
            {
                if (isGolden)
                {
                    Destroy(transform.GetChild(1).gameObject);
                }
                GetComponent<Collider2D>().enabled = false;

                if (!isGolden)
                {
                    int credNo = Random.Range(1, 4);
                    int chance = Random.Range(1, handler.healthChance);

                    if (chance == 1)
                    {
                        float x = Random.Range(-0.5f, 0.5f);
                        float y = Random.Range(-0.5f, 0.5f);

                        GameObject health = Instantiate(handler.hpprefab);

                        health.transform.position = new Vector3(transform.position.x + x, transform.position.y + y);
                    }
                    for (int i = 0; i < credNo; i++)
                    {
                        float x = Random.Range(-0.5f, 0.5f);
                        float y = Random.Range(-0.5f, 0.5f);

                        GameObject cred = Instantiate(handler.credprefab);

                        cred.transform.position = new Vector3(transform.position.x + x, transform.position.y + y);
                    }
                }
                else
                {
                    int credNo = Random.Range(20, 30);
                    int healthNo = Random.Range(1, 3);

                    for (int z = 0; z < healthNo; z++)
                    {
                        float x = Random.Range(-0.5f, 0.5f);
                        float y = Random.Range(-0.5f, 0.5f);

                        GameObject health = Instantiate(handler.hpprefab);

                        health.transform.position = new Vector3(transform.position.x + x, transform.position.y + y);
                    }
                    for (int i = 0; i < credNo; i++)
                    {
                        float x = Random.Range(-0.5f, 0.5f);
                        float y = Random.Range(-0.5f, 0.5f);

                        GameObject cred = Instantiate(handler.credprefab);

                        cred.transform.position = new Vector3(transform.position.x + x, transform.position.y + y);
                    }
                }

                animator.SetBool("Destroyed", true);
                isDestroyed = true;
                transform.GetChild(0).gameObject.SetActive(false);
                GetComponent<BoxCollider2D>().enabled = false;
                
                int sound = Random.Range(0, 2);
                if (sound == 0)
                {
                    Sound.PlaySound("box1");
                }
                else
                {
                    Sound.PlaySound("box2");
                }
            }
        }
        if (isDestroyed && fading == false)
        {
            timer += 0.005f * 60f / (1f / Time.deltaTime); //Change first number for rate;
            if (timer >= 1f)
            {
                fading = true;
            }
        }
        if (fading)
        {
            if (alpha >= 0.005f)
            {
                alpha -= 0.005f * 60f / (1f / Time.deltaTime); //Change first number for rate;
            }
            else
            {
                Destroy(gameObject);
            }

            Color x = GetComponent<SpriteRenderer>().color;
            x.a = alpha;
            GetComponent<SpriteRenderer>().color = x;
        }
    }

    void StopAnimation()
    {
        animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = handler.brokenCrate;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "MeleeSlash(Clone)")
        {
            health -= originalhealth * 0.6f;
        }
        else if (collision.gameObject.name == "WaveBullet(Clone)")
        {
            health -= collision.GetComponent<Projectile>().damage;
        }
        else if (collision.gameObject.name == "HCBullet(Clone)")
        {
            health -= collision.GetComponent<Projectile>().damage;
        }
        else if (collision.gameObject.name == "Explosion(Clone)")
        {
            health -= collision.GetComponent<Explosion>().damage;
        }
        else if (collision.gameObject.tag == "Player")
        {
            health = 0f;
        }
    }
}
