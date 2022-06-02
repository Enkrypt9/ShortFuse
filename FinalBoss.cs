using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    private GameHandler handler;
    private Enemy mainScript;
    private Vector2 tempplayerpos;
    private GameObject hand;
    public int currentAttack;

    public ParticleSystem lockonPE;
    private UnityEngine.ParticleSystem.ShapeModule lockonPEshape;

    public GameObject gunFrame; //Parent object where gun images are attached to
    public GameObject eye; //Empty object where eye is

    private int bossAttackAngle;

    public GameObject minion;
    public GameObject[] spawners;
    public GameObject levelTen; //Level 10 parent object for minions

    public GameObject bullet4;
    public GameObject bullet5;

    // Start is called before the first frame update
    void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        hand = transform.GetChild(0).gameObject;
        mainScript = gameObject.GetComponent<Enemy>();

        lockonPEshape = lockonPE.shape;

        bossAttackAngle = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 new_pos = handler.player.transform.position;

        Vector3 vec = new_pos - lockonPE.transform.position;
        float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;

        lockonPE.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

        float x = handler.player.transform.position.x - lockonPE.transform.position.x;
        float y = handler.player.transform.position.y - lockonPE.transform.position.y;

        float currentDistance = Mathf.Sqrt((x * x) + (y * y));

        lockonPEshape.scale = new Vector2(lockonPE.shape.scale.x, currentDistance);
        lockonPEshape.position = new Vector2(lockonPE.shape.position.x, -0.5f * currentDistance);
        lockonPE.emissionRate = currentDistance * 10;
    }

    public void BounceBossGuns(float amount)
    {
        gunFrame.transform.localPosition = new Vector3(gunFrame.transform.localPosition.x, amount, 0f);
    }

    public void Attack(int attackType, int bannedAttack)
    {
        if (mainScript.bossStage == 1)
        {
            if (attackType <= 35 && bannedAttack != 0)
            {
                currentAttack = 0;
                Idle();
            }
            else if (attackType <= 75 && bannedAttack != 1)
            {
                currentAttack = 1;
                Attack1();

            }
            else if (attackType <= 100 && bannedAttack != 2)
            {
                currentAttack = 2;
                Attack2(4);
            }
        }
        else if (mainScript.bossStage == 2)
        {
            if (attackType <= 20 && bannedAttack != 0)
            {
                currentAttack = 0;
                Idle();
            }
            else if (attackType <= 35 && bannedAttack != 1)
            {
                currentAttack = 1;
                Attack1();
            }
            else if (attackType <= 55 && bannedAttack != 2)
            {
                currentAttack = 2;
                Attack2(6);
            }
            else if (attackType <= 80 && bannedAttack != 3)
            {
                currentAttack = 3;
                Attack3();
            }
            else if (attackType <= 100 && bannedAttack != 4)
            {
                currentAttack = 4;
                Attack4();
            }
        }
        else
        {
            if (attackType <= 10 && bannedAttack != 0)
            {
                currentAttack = 0;
                Idle();
            }
            else if (attackType <= 20 && bannedAttack != 1)
            {
                currentAttack = 1;
                Attack1();
            }
            else if (attackType <= 60 && bannedAttack != 2)
            {
                currentAttack = 2;
                Attack2(8);
            }
            else if (attackType <= 70 && bannedAttack != 3)
            {
                currentAttack = 3;
                Attack3();
            }
            else if (attackType <= 80 && bannedAttack != 4)
            {
                currentAttack = 4;
                Attack4();
            }
            else if (attackType <= 100 && bannedAttack != 5)
            {
                currentAttack = 5;
                Attack5();
            }
        }
    }

    void Idle() //Idle
    {
        mainScript.SetShootTime(mainScript.shootTime * 5);

        if (mainScript.GetAttackStack() <= 0)
        {
            mainScript.SetAttackStack(0);
        }
    }
    void Attack1() //Normal bullets
    {
        for (int i = 0; i < mainScript.bulletCount; i++)
        {
            tempplayerpos = new Vector2(handler.player.transform.position.x, handler.player.transform.position.y);
            GameObject proj = Instantiate(mainScript.bullet, hand.transform.position, Quaternion.identity);
            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
            proj.GetComponent<Projectile>().destination = tempplayerpos;
            proj.GetComponent<Projectile>().ignore = gameObject;

            proj.GetComponent<SpriteRenderer>().color = mainScript.eyeColor;
        }

        mainScript.SetShootTime(mainScript.shootTime);
        if (mainScript.GetAttackStack() <= 0)
        {
            mainScript.SetAttackStack(5);
        }
    }
    void Attack2(int stack) //Fire pools
    {
        int offsetX = Random.Range(-1000, 1000);
        int offsetY = Random.Range(-1000, 1000);

        while (offsetX == 0 && offsetY == 0)
        {
            Debug.Log("BOTH WERE ZERO");
            offsetX = Random.Range(-1000, 1000);
            offsetY = Random.Range(-1000, 1000);
        }

        for (int i = 0; i < 1; i++)
        {
            tempplayerpos = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
            GameObject proj = Instantiate(mainScript.secondaryBullet, hand.transform.position, Quaternion.identity);
            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
            proj.GetComponent<Projectile>().destination = tempplayerpos;
            proj.GetComponent<Projectile>().ignore = gameObject;

            //proj.GetComponent<SpriteRenderer>().color = mainScript.eyeColor;
        }

        mainScript.SetShootTime(mainScript.shootTime / 2);
        if (mainScript.GetAttackStack() <= 0)
        {
            mainScript.SetAttackStack(stack);
        }
    }
    void Attack3() //Circle Spray
    {
        if (bossAttackAngle < 360)
        {
            if (mainScript.bossStage == 2)
            {
                bossAttackAngle += 15;
            }
            else
            {
                bossAttackAngle += 8;
            }
        }
        else
        {
            bossAttackAngle = 0;
        }

        for (int i = 0; i < 4; i++)
        {

            float x = 1 * Mathf.Cos(Mathf.Deg2Rad * (bossAttackAngle + (i * 90f)));
            float y = 1 * Mathf.Sin(Mathf.Deg2Rad * (bossAttackAngle + (i * 90f)));

            tempplayerpos = new Vector2(transform.position.x + x, transform.position.y + y);
            GameObject proj = Instantiate(mainScript.tertiaryBullet, transform.position, Quaternion.identity);
            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
            proj.GetComponent<Projectile>().destination = tempplayerpos;
            proj.GetComponent<Projectile>().ignore = gameObject;

            //proj.GetComponent<SpriteRenderer>().color = eyeColor;
        }

        if (mainScript.bossStage == 2)
        {
            mainScript.SetShootTime(mainScript.shootTime / 20);
        }
        else
        {
            mainScript.SetShootTime(mainScript.shootTime / 40);
        }

        if (mainScript.GetAttackStack() <= 0)
        {
            if (mainScript.bossStage == 2)
            {
                mainScript.SetAttackStack(48);
            }
            else
            {
                mainScript.SetAttackStack(45);
            }
        }
    }
    void Attack4() //Spawn robots
    {
        //1 2
        //3 4

        int x = 0; //Which spawner to spawn at

        if (handler.player.transform.position.x < transform.position.x  &&
            handler.player.transform.position.y > transform.position.y)
        {
            //1
            x = 3;
        }
        else if (handler.player.transform.position.x > transform.position.x &&
            handler.player.transform.position.y > transform.position.y)
        {
            //2
            x = 2;
        }
        else if (handler.player.transform.position.x < transform.position.x &&
            handler.player.transform.position.y < transform.position.y)
        {
            //3
            x = 1;
        }
        else if (handler.player.transform.position.x > transform.position.x &&
            handler.player.transform.position.y < transform.position.y)
        {
            //4
            x = 0;
        }

        GameObject minionRobot = Instantiate(minion, spawners[x].transform.position, Quaternion.identity);
        minionRobot.transform.SetParent(levelTen.transform);

        mainScript.SetShootTime(mainScript.shootTime * 2);
        if (mainScript.GetAttackStack() <= 0)
        {
            mainScript.SetAttackStack(3);
        }
    }
    void Attack5() //Lock On
    {
        lockonPE.gameObject.SetActive(true);
        Invoke("ShootDeathBullet", 1f);

        mainScript.SetShootTime(mainScript.shootTime * 3);
        if (mainScript.GetAttackStack() <= 0)
        {
            mainScript.SetAttackStack(0);
        }
    }

    void ShootDeathBullet()
    {
        lockonPE.gameObject.SetActive(false);

        for (int i = 0; i < 1; i++)
        {
            tempplayerpos = new Vector2(handler.player.transform.position.x, handler.player.transform.position.y);
            GameObject proj = Instantiate(bullet4, eye.transform.position, Quaternion.identity);
            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
            proj.GetComponent<Projectile>().destination = tempplayerpos;
            proj.GetComponent<Projectile>().ignore = gameObject;

            proj.GetComponent<SpriteRenderer>().color = mainScript.eyeColor;
        }
    }
}
