using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverObject : MonoBehaviour
{
    private GameHandler handler;

    public int type; //0 = nothing, 1 = brotherCrown
                     //2 = GoldenKatana

    public int coverNumber;
    public GameObject explosionPrefab;

    private void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (type == 0)
            {
                handler.settings.GetComponent<Settings>().easterEggs[coverNumber - 1] = true;
                handler.InitiateEEPopUp(coverNumber - 1);
                InvokeRepeating("FadeOut", 0f, 0.01f);
            }
            else if (type == 1)
            {
                handler.player.GetComponent<PlayerMove>().brother.GetComponent<SoulFollow>().ActivateCrown();

                handler.settings.GetComponent<Settings>().foundCrown = true;

                GameObject explosion = Instantiate(handler.enemyExplosion, transform.GetChild(0).transform.position, Quaternion.identity);
                explosion.GetComponent<Explosion>().damage = 0;

                GameObject miniExplosion = Instantiate(handler.enemyExplosion, transform.GetChild(0).position, Quaternion.identity);
                miniExplosion.transform.GetChild(0).localScale = new Vector3(2f, 2f, 2f);
                miniExplosion.GetComponent<Explosion>().damage = 0;

                Destroy(gameObject);
            }
            else if (type == 2)
            {

                handler.settings.GetComponent<Settings>().foundKatana = true;

                handler.guns7 = handler.goldKatana;
                handler.allGuns[6] = handler.guns7;

                GameObject explosion = Instantiate(handler.enemyExplosion, transform.GetChild(0).transform.position, Quaternion.identity);
                explosion.transform.GetChild(0).localScale = new Vector3(3f, 3f, 3f);
                explosion.GetComponent<Explosion>().damage = 0;

                Destroy(gameObject);
            }
            else if (type == 3)
            {
                handler.settings.GetComponent<Settings>().easterEggs[coverNumber - 1] = true;
                handler.InitiateEEPopUp(coverNumber - 1);

                explosionPrefab.SetActive(true);
                Sound.PlaySound("explosion");

                Destroy(gameObject);
            }
        }
    }

    void FadeOut()
    {
        Color x = GetComponent<SpriteRenderer>().color;
        if (x.a > 0.005f)
        {
            x.a -= 0.005f;
        }
        else
        {
            x.a = 0f;
        }

        GetComponent<SpriteRenderer>().color = x;

        if (x.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
