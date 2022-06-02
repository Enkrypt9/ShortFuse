using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameHandler handler;

    public float speed;
    public int damage;
    public int PType; //1 = Player proj, 2 = Enemy proj
    public GameObject enemy;
    public GameObject ignore;

    public Vector2 destination;
    public Vector3 prev_pos;

    public Rigidbody2D rb;

    public Sprite[] directions;

    public float spread;

    public bool melee;

    private Animator animator;

    public bool isCritical;

    public float angle;

    public float angleOffset;

    public int startLives; //How many enemies it can hit before death
    private int lives; //How many more enemies it can hit before death

    public bool isHoming;
    public float homingSwitchTime; //How often to redirect to target
    public float startHomingTime; //After how long to start homing
    public float stopHomingTime; //After how long to stop homing
    public GameObject homingTarget;

    private float homingTimer;

    // Start is called before the first frame update
    void Start()
    {
        Color someColor = GetComponent<SpriteRenderer>().color;
        someColor.a = 0f;
        GetComponent<SpriteRenderer>().color = someColor;

        Invoke("Appear", 0.05f);

        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = destination.normalized * speed;

        if (melee && gameObject.name != "MeleeBossBigSword(Clone)")
        {
            animator = GetComponent<Animator>();
            if (handler.settings.GetComponent<Settings>().foundKatana)
            {
                int slash = Random.Range(4, 7);
                animator.SetInteger("slashtype", slash);
            }
            else
            {
                int slash = Random.Range(1, 4);
                animator.SetInteger("slashtype", slash);
            }
        }
        else if (gameObject.name == "WaveBullet(Clone)")
        {
            Invoke("Kill", 0.5f);
        }
        else if (gameObject.name == "MeleeBossBigSword(Clone)")
        {
            InvokeRepeating("SlowDown", 0f, 0.01f);
        }
        else if (gameObject.name == "PoisonBossWave(Clone)")
        {
            Invoke("CreatePoisonPool", 0.5f);
        }
        else if (gameObject.name == "UBossWave(Clone)")
        {
            float explodeTime = Random.Range(1f, 2.2f);
            Invoke("CreateFirePool", explodeTime);
        }
        else if (gameObject.name == "DBossCluster(Clone)")
        {
            Invoke("CreateCluster", 2f);
            Invoke("Kill", 2f);
        }

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        rb.velocity = new Vector3(rb.velocity.x + x, rb.velocity.y + y);

        prev_pos = transform.position;
        
        lives = startLives;

        if (enemy != null) //Never happens
        {
            destination = enemy.transform.position - transform.position;
        }

        if (PType == 1)
        {
            int crit = Random.Range(0, 100);

            if (crit < handler.critChance)
            {
                isCritical = true;
                damage = damage * 3;
            }
            else
            {
                isCritical = false;
            }
        }
        else
        {
            isCritical = false;
        }

        if (isHoming)
        {
            isHoming = false;

            Invoke("StartHoming", startHomingTime);

            if (stopHomingTime != 0f)
            {
                Invoke("StopHoming", stopHomingTime + startHomingTime);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isHoming)
        {
            homingTimer += 0.0005f * 60f / (1f / Time.deltaTime); //Change first number for rate
            Debug.Log(homingTimer);

            if (homingTimer >= homingSwitchTime)
            {
                Debug.Log("HOMED");

                destination = new Vector2(homingTarget.transform.position.x, homingTarget.transform.position.y) - new Vector2(transform.position.x, transform.position.y);

                rb.velocity = destination.normalized * speed;

                //transform.LookAt(homingTarget.transform);

                //transform.rotation = Quaternion.LookRotation(homingTarget.transform.forward);

                /*if (prev_pos != transform.position)
                {
                    Vector3 new_pos = transform.position;

                    Vector3 vec = new_pos - prev_pos;
                    angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    prev_pos = transform.position;
                }
                */


                homingTimer = 0f;
            }

        }
        if (gameObject.name == "MeleeBossSword(Clone)")
        {
            Vector3 new_pos = transform.position;

            Vector3 vec = new_pos - prev_pos;
            angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + angleOffset, Vector3.forward);
            angleOffset -= 20f;
        }
        else if (gameObject.name == "MeleeBossBigSword(Clone)")
        {
            Vector3 new_pos = transform.position;

            Vector3 vec = new_pos - prev_pos;
            angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 70f, Vector3.forward);
        }
        else
        {
            if (!isHoming)
            {
                Vector3 new_pos = transform.position;

                Vector3 vec = new_pos - prev_pos;
                angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                Vector3 vectorToTarget = homingTarget.transform.position - transform.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);

            }
        }

        if ((rb.velocity.x > 0 && rb.velocity.x < 3) || (rb.velocity.x < 0 && rb.velocity.x > -3))
        {
            if ((rb.velocity.y > 0 && rb.velocity.y < 3) || (rb.velocity.y < 0 && rb.velocity.y > -3))
            {
                if (gameObject.name != "MeleeBossBigSword(Clone)" && gameObject.name != "EMP(Clone)")
                {
                    Debug.Log("DESTROYED: " + gameObject.name);
                    Destroy(gameObject);
                }
            }
        }
        if (ignore != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), ignore.GetComponent<Collider2D>());
            ignore = null;
        }

        /*
        if (rb.velocity.x > 0) //Right
        {
            if (rb.velocity.y > 0) //Upper Right
            {
                if (rb.velocity.x > rb.velocity.y)
                {
                    GetComponent<SpriteRenderer>().sprite = directions[1];
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = directions[0];
                }
            }
            else //Lower Right
            {
                if (rb.velocity.x > -rb.velocity.y)
                {
                    GetComponent<SpriteRenderer>().sprite = directions[1];
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = directions[2];
                }
            }
        }
        else //Left
        {
            if (rb.velocity.y > 0) //Upper Left
            {
                if (rb.velocity.x < rb.velocity.y)
                {
                    GetComponent<SpriteRenderer>().sprite = directions[3];
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = directions[0];
                }
            }
            else //Lower Left
            {
                if (rb.velocity.x < -rb.velocity.y)
                {
                    GetComponent<SpriteRenderer>().sprite = directions[3];
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = directions[2];
                }
            }
        }
        */

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPosition.x < -handler.screen_bound || screenPosition.x > (Screen.width + handler.screen_bound)
            || screenPosition.y < -handler.screen_bound || screenPosition.y > (Screen.height + handler.screen_bound))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != ignore && gameObject.name != "EMP(Clone)")
        {
            if (collision.gameObject.tag == "Projectile")
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
            }
            else
            {
                if (PType == 1)
                {
                    if (gameObject.name == "RLBullet(Clone)")
                    {
                        Instantiate(handler.explosion, collision.contacts[0].point, Quaternion.identity);
                        Sound.PlaySound("explosion");
                    }
                    if (collision.gameObject.tag == "Enemy")
                    {
                        if (gameObject.name == "PistolBullet(Clone)")
                        {
                            handler.ultValue += 0.4f; //0.4
                        }
                        else if (gameObject.name == "MinigunBullet(Clone)")
                        {
                            handler.ultValue += 0.2f;
                        }
                        else if (gameObject.name == "ShotgunBullet(Clone)")
                        {
                            handler.ultValue += 0.2f;
                        }
                        else if (gameObject.name == "WaveBullet(Clone)")
                        {      
                            //In Trigger
                        }
                        else if (gameObject.name == "HCBullet(Clone)")
                        {
                            //In Trigger
                        }
                        else if (gameObject.name == "RLBullet(Clone)")
                        {
                            handler.ultValue += 3.5f;
                        }
                        else if (gameObject.name == "MeleeSlash(Clone)")
                        {
                            //In Trigger
                        }

                        Vector3 vec = transform.position - prev_pos;
                        float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                        if (handler.gunEquipped == 0 || handler.gunEquipped == 1)
                        {
                            GameObject spark = Instantiate(handler.coneSparks, collision.transform.position, Quaternion.identity);
                            spark.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
                        }
                        else if (handler.gunEquipped == 2)
                        {
                            GameObject spark = Instantiate(handler.ShottySparks, collision.transform.position, Quaternion.identity);
                            spark.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
                        }

                        ParticleSystem debris = Instantiate(handler.debris1, collision.transform.position, Quaternion.identity);
                        ParticleSystem genericSpark = Instantiate(handler.sparkPE, collision.transform.position, Quaternion.identity);
                        if (collision.gameObject.GetComponent<Enemy>() != null)
                        {
                            debris.transform.localScale *= collision.gameObject.GetComponent<Enemy>().particleScale;
                            genericSpark.transform.localScale *= collision.gameObject.GetComponent<Enemy>().particleScale;
                        }

                        if (collision.gameObject.GetComponent<Enemy>() != null)
                        {
                            if (!collision.gameObject.GetComponent<Enemy>().isInvicible)
                            {
                                collision.gameObject.GetComponent<Enemy>().health -= damage;

                                GameObject popup = Instantiate(handler.dmgPopUp, collision.transform.position, Quaternion.identity);
                                popup.GetComponent<TextPopUp>().text = ((int)damage).ToString();
                                popup.GetComponent<TextPopUp>().target = collision.gameObject;
                                popup.GetComponent<TextPopUp>().damage = (int)damage;

                                if (isCritical)
                                {
                                    handler.crosshairState = 2;
                                    popup.GetComponent<TextPopUp>().crit = true;
                                }
                                else
                                {
                                    handler.crosshairState = 1;
                                }
                            }
                        }

                        //
                        bool hasHB = false;

                        foreach (Transform child in collision.transform)
                        {
                            if (child.name == "EnemyHealthBar(Clone)")
                            {
                                hasHB = true;
                            }
                        }

                        if (!hasHB && (collision.gameObject.GetComponent<Rigidbody2D>() != null))
                        {
                            GameObject HB = Instantiate(handler.enemyHealthBar, new Vector3(collision.transform.position.x, collision.transform.position.y - (1f * collision.gameObject.transform.localScale.x)), Quaternion.identity);
                            HB.transform.localScale = collision.transform.localScale;
                            HB.GetComponent<EnemyHB>().anchor = collision.gameObject;

                            HB.transform.SetParent(collision.gameObject.transform);
                        }
                        //
                        
                        Destroy(gameObject);
                    }
                    else if (collision.gameObject.tag == "Player")
                    {
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
                    }
                    else if (collision.gameObject.tag == "Crate")
                    {
                        collision.gameObject.GetComponent<Crate>().health -= damage;
                        Destroy(gameObject);
                    }
                    else
                    {
                        ContactPoint2D contact = collision.contacts[0];
                        Vector3 pos = contact.point;
                        //Instantiate(handler.bulletHole, pos, Quaternion.identity);

                        Destroy(gameObject);
                    }
                }
                else if (PType == 2)
                {
                    if (gameObject.name == "DBossCluster(Clone)")
                    {
                        Invoke("CreateCluster", 0f);
                    }
                    if (collision.gameObject.tag == "Player")
                    {
                        if (gameObject.name == "RPGBossBullet(Clone)")
                        {
                            Instantiate(handler.enemyExplosion, transform.position, Quaternion.identity);
                            Sound.PlaySound("explosion");
                        }
                        if (gameObject.name == "UBossWave(Clone)")
                        {
                            CreateFirePool();
                            Sound.PlaySound("explosion");
                        }

                        if (!handler.ultTransition && !handler.player.GetComponent<PlayerMove>().isDashing)
                        {
                            Vector3 new_pos = transform.position;

                            Vector3 vec = new_pos - handler.player.transform.position;
                            float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;

                            Instantiate(handler.hitIndicator, handler.player.transform.position, Quaternion.AngleAxis(angle + 90, Vector3.forward));

                            handler.playerHitTimer = 0;
                            collision.gameObject.GetComponent<PlayerMove>().handler.health -= damage;
                        }

                        Destroy(gameObject);

                    }
                    else if (collision.gameObject.tag == "Enemy")
                    {
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
                    }
                    else
                    {
                        if (gameObject.name == "RPGBossBullet(Clone)")
                        {
                            Instantiate(handler.enemyExplosion, transform.position, Quaternion.identity);
                            Sound.PlaySound("explosion");
                        }
                        else if (gameObject.name == "MiniBossRPG(Clone)")
                        {
                            Instantiate(handler.enemyExplosion, transform.position, Quaternion.identity);
                            Sound.PlaySound("explosion");
                        }
                        else if (gameObject.name == "UBossWave(Clone)")
                        {
                            CreateFirePool();
                            Sound.PlaySound("explosion");
                        }

                        Destroy(gameObject);
                    }
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.name == "WaveBullet(Clone)")
        {
            if (collision.gameObject.tag == "Wall")
            {
                Destroy(gameObject);
            }
            if (collision.gameObject.tag == "Enemy")
            {
                handler.ultValue += 0.9f;
                ParticleSystem debris = Instantiate(handler.debris1, collision.transform.position, Quaternion.identity);
                ParticleSystem genericSpark = Instantiate(handler.sparkPE, collision.transform.position, Quaternion.identity);
                if (collision.gameObject.GetComponent<Enemy>() != null)
                {
                    debris.transform.localScale *= collision.gameObject.GetComponent<Enemy>().particleScale;
                    genericSpark.transform.localScale *= collision.gameObject.GetComponent<Enemy>().particleScale;
                }

                //
                bool hasHB = false;

                foreach (Transform child in collision.transform)
                {
                    if (child.name == "EnemyHealthBar(Clone)")
                    {
                        hasHB = true;
                    }
                }

                if (!hasHB && (collision.gameObject.GetComponent<Rigidbody2D>() != null))
                {
                    GameObject HB = Instantiate(handler.enemyHealthBar, new Vector3(collision.transform.position.x, collision.transform.position.y - (1f * collision.gameObject.transform.localScale.x)), Quaternion.identity);
                    HB.transform.localScale = collision.transform.localScale;
                    HB.GetComponent<EnemyHB>().anchor = collision.gameObject;

                    HB.transform.SetParent(collision.gameObject.transform);
                }
                //
            }
        }
        else if (gameObject.name == "MeleeSlash(Clone)")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                handler.ultValue += 0.4f;
                ParticleSystem debris = Instantiate(handler.debris1, collision.transform.position, Quaternion.identity);
                ParticleSystem genericSpark = Instantiate(handler.sparkPE, collision.transform.position, Quaternion.identity);
                ParticleSystem slash = Instantiate(handler.slashPE, collision.transform.position, Quaternion.identity);
                if (collision.gameObject.GetComponent<Enemy>() != null)
                {
                    debris.transform.localScale *= collision.gameObject.GetComponent<Enemy>().particleScale;
                    genericSpark.transform.localScale *= collision.gameObject.GetComponent<Enemy>().particleScale;
                    slash.transform.localScale *= collision.gameObject.GetComponent<Enemy>().particleScale;
                }

                //
                bool hasHB = false;

                foreach (Transform child in collision.transform)
                {
                    if (child.name == "EnemyHealthBar(Clone)")
                    {
                        hasHB = true;
                    }
                }

                if (!hasHB && (collision.gameObject.GetComponent<Rigidbody2D>() != null))
                {
                    GameObject HB = Instantiate(handler.enemyHealthBar, new Vector3(collision.transform.position.x, collision.transform.position.y - (1f * collision.gameObject.transform.localScale.x)), Quaternion.identity);
                    HB.transform.localScale = collision.transform.localScale;
                    HB.GetComponent<EnemyHB>().anchor = collision.gameObject;

                    HB.transform.SetParent(collision.gameObject.transform);
                }
                //
            }
        }
        else if (gameObject.name == "HCBullet(Clone)")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                transform.GetChild(0).GetComponent<EnemyRadiusDetection>().alreadyHit.Add(collision.gameObject);

                lives -= 1;
                if (lives <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    if (transform.GetChild(0).GetComponent<EnemyRadiusDetection>().GetNearestEnemy(transform.position) != null)
                    {
                        Invoke("Ricochet", 0.01f);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }

                handler.ultValue += 0.9f;
                ParticleSystem debris = Instantiate(handler.debris1, collision.transform.position, Quaternion.identity);
                ParticleSystem genericSpark = Instantiate(handler.sparkPE, collision.transform.position, Quaternion.identity);
                if (collision.gameObject.GetComponent<Enemy>() != null)
                {
                    debris.transform.localScale *= collision.gameObject.GetComponent<Enemy>().particleScale;
                    genericSpark.transform.localScale *= collision.gameObject.GetComponent<Enemy>().particleScale;
                }
                //Instantiate(handler.slashPE, collision.transform.position, Quaternion.identity);

                //
                bool hasHB = false;

                foreach (Transform child in collision.transform)
                {
                    if (child.name == "EnemyHealthBar(Clone)")
                    {
                        hasHB = true;
                    }
                }

                if (!hasHB && (collision.gameObject.GetComponent<Rigidbody2D>() != null))
                {
                    GameObject HB = Instantiate(handler.enemyHealthBar, new Vector3(collision.transform.position.x, collision.transform.position.y - (1f * collision.gameObject.transform.localScale.x)), Quaternion.identity);
                    HB.transform.localScale = collision.transform.localScale;
                    HB.GetComponent<EnemyHB>().anchor = collision.gameObject;

                    HB.transform.SetParent(collision.gameObject.transform);
                }
                //
            }
            if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Crate") //|| collision.gameObject.tag == "Shop")
            {
                if (collision.gameObject.layer == 13)
                {
                    GetComponent<Collider2D>().isTrigger = false;
                    Invoke("Kill", 0.02f);
                }
                else if (collision.gameObject.tag == "Crate")
                {
                    transform.GetChild(0).GetComponent<EnemyRadiusDetection>().alreadyHit.Add(collision.gameObject);

                    lives -= 1;
                    if (lives <= 0)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        if (transform.GetChild(0).GetComponent<EnemyRadiusDetection>().GetNearestEnemy(transform.position) != null)
                        {
                            Invoke("Ricochet", 0.01f);
                        }
                        else
                        {
                            Destroy(gameObject);
                        }
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (melee)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (!handler.ultTransition && !handler.player.GetComponent<PlayerMove>().isDashing)
                {
                    Instantiate(handler.slashPE, collision.transform.position, Quaternion.identity);

                    Vector3 new_pos = transform.position;

                    Vector3 vec = new_pos - handler.player.transform.position;
                    float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;

                    Instantiate(handler.hitIndicator, handler.player.transform.position, Quaternion.AngleAxis(angle + 90, Vector3.forward));

                    handler.playerHitTimer = 0;
                    collision.gameObject.GetComponent<PlayerMove>().handler.health -= damage;
                }
            }
        }
    }
    void Ricochet()
    {
        if (transform.GetChild(0).GetComponent<EnemyRadiusDetection>().GetNearestEnemy(transform.position) != null)
        {
            prev_pos = transform.position;
            destination = transform.GetChild(0).GetComponent<EnemyRadiusDetection>().GetNearestEnemy(transform.position).transform.position - transform.position;
            rb.velocity = destination.normalized * speed;
        }
    }
    public void Kill()
    {
        Destroy(gameObject);
    }
    public void InvokeSpawnSiblings()
    {
        if (angle != 0)
        {
            Invoke("SpawnSiblings", 0.01f);
        }
        else
        {
            Invoke("InvokeSpawnSiblings", 0f);
        }
    }
    void SpawnSiblings()
    {
        for (int i = 0; i < 2; i++)
        {
            float radius = Mathf.Sqrt(destination.x * destination.x + destination.y * destination.y);

            float finalAngle;

            if (i == 0)
            {
                finalAngle = angle + 8f;
            }
            else
            {
                finalAngle = angle - 8f;
            }

            //finalAngle = angle + 1f * i;

            //Debug.Log(angle);
            Vector2 destination2 = new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * finalAngle), radius * Mathf.Sin(Mathf.Deg2Rad * finalAngle));
            //Debug.Log(destination2);

            GameObject waveproj = Instantiate(handler.bullets[3], prev_pos, Quaternion.identity);
            waveproj.GetComponent<Projectile>().destination = destination2;
            waveproj.GetComponent<Projectile>().ignore = gameObject;
        }
    }
    
    void Appear()
    {
        Color someColor = GetComponent<SpriteRenderer>().color;
        someColor.a = 1f;
        GetComponent<SpriteRenderer>().color = someColor;
    }

    void SlowDown()
    {
        bool stoppedX = false;
        bool stoppedY = false;

        if (rb.velocity.x > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x - 1f, rb.velocity.y);

            if (rb.velocity.x < 0)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
                stoppedX = true;
            }
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x + 1f, rb.velocity.y);

            if (rb.velocity.x > 0)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
                stoppedX = true;
            }
        }
        if (rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 1f);

            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                stoppedY = true;
            }
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 1f);

            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                stoppedY = true;
            }
        }

        if (stoppedX && stoppedY)
        {
            Invoke("Redirect", 1f);
            CancelInvoke("SlowDown");
        }
    }
    void Redirect()
    {
        prev_pos = transform.position;
        destination = handler.player.transform.position - transform.position;
        rb.velocity = destination.normalized * speed;
    }

    void CreatePoisonPool()
    {
        GameObject poison = Instantiate(handler.poisonPool, transform.position, Quaternion.identity);
        poison.transform.position = new Vector3(poison.transform.position.x, poison.transform.position.y, 0f);
        Destroy(gameObject);
    }
    void CreateFirePool()
    {
        GameObject fire = Instantiate(handler.firePool, transform.position, Quaternion.identity);
        fire.transform.position = new Vector3(fire.transform.position.x, fire.transform.position.y, 0f);
        Destroy(gameObject);
    }

    void CreateCluster()
    {
        Sound.PlaySound("explosion");

        Vector2 tempplayerpos = Vector2.zero;

        for (int i = 0; i < 36; i++)
        {
            float x = 1 * Mathf.Cos(Mathf.Deg2Rad * (i * 10));
            float y = 1 * Mathf.Sin(Mathf.Deg2Rad * (i * 10));

            tempplayerpos = new Vector2(transform.position.x + x, transform.position.y + y);
            GameObject proj = Instantiate(handler.clusterBullet, transform.position, Quaternion.identity);
            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
            proj.GetComponent<Projectile>().destination = tempplayerpos;
            proj.GetComponent<Projectile>().ignore = gameObject;

            proj.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        }
    }

    void StartHoming()
    {
        isHoming = true;
    }
    void StopHoming()
    {
        prev_pos = transform.position;
        isHoming = false;
    }
}
