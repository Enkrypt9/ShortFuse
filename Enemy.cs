using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private GameHandler handler;
    private GameObject player;

    public float health;
    public float originalhealth;

    public int damage;

    //public float speed;
    private Vector2 movement;
    private Vector3 direction;

    private GameObject hand;
    private GameObject gun;

    public GameObject bullet;
    public GameObject secondaryBullet;
    public GameObject tertiaryBullet;
    private Vector2 tempplayerpos;
    public float shootTime;
    private float shootTimer;

    public int gunEquipped;
    public int bulletCount;

    private Vector2 distance;

    public bool moveDisabled;
    public bool isShooting;

    [SerializeField] private int level;

    public bool inRange;

    public bool TwoKeys;
    public bool animSwitched;
    public int DiagSwapTime;
    private float DiagSwapTimer;

    public Animator animator;
    public Sprite[] allEnemyGuns;

    public bool touchingPlayer;

    public bool isRanged;

    private bool hasSeen; //Has the player been seen by the boss

    private bool stunned;

    public GameObject target;
    private NavMeshAgent agent;

    public bool permaDisabled;

    public bool isInvicible;

    public bool isBoss;
    public int bossNumber;
    public bool bossWakingUp;
    public bool bossWokenUp;

    public bool stalkPlayer; //Continue to follow player after going off screen

    public bool destroyer; //Destroy spires and crates on collision

    public bool colorChangeable;
    public Texture2D sprites;

    public Color[] colors;

    public Color eyeColor;
    public Color bodyColor;
    public Color body2Color;
    public Color headColor;
    public Color head2Color;

    private SpriteRenderer spRend;

    private int attackType;
    private int attackStack; //Attack will be played again this many times
    private int bannedAttack; //Attack that will not be played next
    private int currentAttack; //Attack that is currently in use
    private int bannedCounter; //Number of frames that the banned attack is banned for

    private int bossAttackAngle = 0;

    private bool pausedForAttack;

    public ParticleSystem[] bossParticles;
    public int bossStage;

    public Color originalEyeColor;

    private float rainbowHue; //Float for DBoss eye colour
    private bool rainbowUp; //Whether hue is increasing or decreasing

    public float particleScale; //Spark + Debris scale per enemy

    void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        player = handler.player;

        spRend = GetComponent<SpriteRenderer>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        hand = transform.GetChild(0).gameObject;
        gun = transform.GetChild(1).gameObject;

        originalEyeColor = eyeColor;

        if (gameObject.name == "UltimateBoss")
        {
            particleScale = 5f;
        }
        else
        {
            particleScale = 1f;
        }

        if (!permaDisabled)
        {
            moveDisabled = false;
        }

        shootTimer = shootTime;

        touchingPlayer = false;

        hasSeen = false;

        target = player;

        if (transform.parent.gameObject.name.Length == 6)
        {
            level = int.Parse(transform.parent.gameObject.name.Substring(transform.parent.gameObject.name.Length - 1));
        }
        else if (transform.parent.gameObject.name.Length == 7)
        {
            level = 10;
        }

        if (!isRanged)
        {
            allEnemyGuns = handler.guns14;
            hand.transform.localPosition = new Vector3(0f, -0.6f);
            gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
            gun.transform.localPosition = new Vector3(0f, -0.344f);

            gun.GetComponent<SpriteRenderer>().color = eyeColor;
        }

        if (isBoss)
        {
            eyeColor = new Color(0, 0, 0);
            bossStage = 1;
        }
        else
        {
            bossStage = 0;

            if (isRanged)
            {
                health = handler.robotHealth[level - 1];
                damage = handler.robotDamage[level - 1];
            }
            else
            {
                health = handler.meleeBotHealth[level - 1];
                damage = handler.meleeBotDamage[level - 1];
            }
        }

        bossWokenUp = false;

        originalhealth = health;

        rainbowHue = 0f;
        rainbowUp = true;
    }

    private void LateUpdate()
    {
        if (colorChangeable && inRange) //Not Efficient
        {
            Texture2D copiedTexture = spRend.sprite.texture;
            Texture2D texture = new Texture2D(copiedTexture.width, copiedTexture.height);

            texture.filterMode = FilterMode.Point;

            for (int x = 0; x < copiedTexture.width; x++)
            {
                for (int y = 0; y < copiedTexture.height; y++)
                {
                    if (copiedTexture.GetPixel(x, y) == colors[0]) //Eyes
                    {
                        texture.SetPixel(x, y, eyeColor);
                    }
                    else if (copiedTexture.GetPixel(x, y) == colors[1]) //Body
                    {
                        texture.SetPixel(x, y, bodyColor);
                    }
                    else if (copiedTexture.GetPixel(x, y) == colors[2]) //Body Shadows
                    {
                        texture.SetPixel(x, y, body2Color);
                    }
                    else if (copiedTexture.GetPixel(x, y) == colors[3]) //Head
                    {
                        texture.SetPixel(x, y, headColor);
                    }
                    else if (copiedTexture.GetPixel(x, y) == colors[4]) //Headband
                    {
                        texture.SetPixel(x, y, head2Color);
                    }
                    else
                    {
                        texture.SetPixel(x, y, copiedTexture.GetPixel(x, y));
                    }
                }
            }

            texture.Apply();

            Sprite pixelSprite = Sprite.Create(texture, spRend.sprite.rect, new Vector2(0.5f, 0.5f), 16);
            spRend.sprite = pixelSprite;
        }
    }
    void Update()
    {
        if (isRanged) //Not efficient
        {
            allEnemyGuns = handler.enemyGuns[gunEquipped];
        }

        if (gameObject != null && !moveDisabled)
        {
            agent.SetDestination(target.transform.position);
        }

        if (moveDisabled || !inRange)
        {
            agent.isStopped = true;
        }

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (!isRanged)
        {
            if (inRange)
            {
                if (transform.position.x > handler.player.transform.position.x - 2f && transform.position.x < handler.player.transform.position.x + 2f
                    && transform.position.y > handler.player.transform.position.y - 2f && transform.position.y < handler.player.transform.position.y + 2f)
                {
                    touchingPlayer = true;
                }
                else
                {
                    touchingPlayer = false;
                }
            }
            else
            {
                touchingPlayer = false;
            }

            if (isBoss && bossNumber == 2)
            {
                if (currentAttack == 2 || currentAttack == 3)
                {
                    Color tmpcolor = gun.GetComponent<SpriteRenderer>().color;
                    tmpcolor.a = 0f;
                    gun.GetComponent<SpriteRenderer>().color = tmpcolor;
                }
                else
                {
                    Color tmpcolor = gun.GetComponent<SpriteRenderer>().color;
                    tmpcolor.a = 1f;
                    gun.GetComponent<SpriteRenderer>().color = tmpcolor;
                }
            }
        }


        if (screenPosition.x < -100 || screenPosition.x > (Screen.width + 100)
            || screenPosition.y < -100 || screenPosition.y > (Screen.height + 100))
        {
            if (stalkPlayer)
            {
                if (hasSeen)
                {
                    inRange = true;
                }
                else
                {
                    inRange = false;
                }
            }
            else
            {
                inRange = false;
            }
        }
        else
        {
            if (!hasSeen)
            {
                if (isBoss)
                {
                    permaDisabled = true;
                    Invoke("BeginBossWakeUp", 1f);
                    handler.player.GetComponent<PlayerMove>().followcam.GetComponent<FollowCamera>().StartBossMove(gameObject);

                    if (bossNumber == 10)
                    {
                        handler.mainCamera.GetComponent<CameraShake>().Shake(0.05f, 5f);
                    }
                }
                hasSeen = true;
            }
            inRange = true;
        }

        if (permaDisabled)
        {
            moveDisabled = true;
            isShooting = false;
        }

        if (pausedForAttack)
        {
            gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];
            hand.transform.localPosition = new Vector3(0f, -0.6f);
            gun.transform.localPosition = new Vector3(0f, -0.344f);

            moveDisabled = true;
            
            if (isBoss && bossNumber != 2)
            {
                isShooting = false;
            }
        }

        if (handler.currentLevel != level || handler.health <= 0)
        {
            moveDisabled = true;
            isInvicible = true;
        }
        else
        {
            if (!bossWakingUp)
            {
                if (inRange)
                {
                    isInvicible = false;
                }
                else
                {
                    isInvicible = true;
                }
            }

            if (!stunned)
            {
                if (!permaDisabled && !pausedForAttack)
                {
                    moveDisabled = false;
                }
            }
        }

        if (!moveDisabled)
        {
            if (inRange)
            {
                direction = (player.transform.position - transform.position).normalized;

                agent.isStopped = false;

            }
        }
        else
        {
            direction = Vector3.zero;
        }

        //Boss Stages
        if (isBoss)
        {
            if (bossNumber == 2)
            {
                if (bossStage < 2)
                {
                    if(health < (originalhealth / 2))
                    {
                        bossStage = 2;
                        bossParticles[0].gameObject.SetActive(true);
                        bossParticles[1].gameObject.SetActive(true);
                        shootTime = shootTime * 0.75f;
                        Sound.PlaySound("explosion");
                    }
                }
            }
            else if (bossNumber == 4)
            {
                if (bossStage < 2)
                {
                    if (health < (originalhealth / 2))
                    {
                        bossStage = 2;
                        bossParticles[0].gameObject.SetActive(true);
                        bossParticles[1].gameObject.SetActive(true);
                        agent.speed = 8;
                        shootTime = 0.3f;
                        Sound.PlaySound("explosion");
                    }
                }
            }
            else if (bossNumber == 6)
            {
                if (bossStage < 2)
                {
                    if (health < (originalhealth / 2))
                    {
                        bossStage = 2;
                        //bossParticles[0].gameObject.SetActive(true);
                        //bossParticles[1].gameObject.SetActive(true);
                        agent.speed = 6;
                        shootTime = 0.3f;
                        Sound.PlaySound("explosion");
                    }
                }
            }
            else if (bossNumber == 7)
            {
                if (bossStage < 2)
                {
                    if (health < (originalhealth / 2))
                    {
                        bossStage = 2;
                        //bossParticles[0].gameObject.SetActive(true);
                        //bossParticles[1].gameObject.SetActive(true);
                        agent.speed = 5;
                        shootTime = 0.15f;
                        Sound.PlaySound("explosion");
                    }
                }
            }
            else if (bossNumber == 8)
            {
                if (bossStage < 2)
                {
                    if (health < (originalhealth / 2))
                    {
                        bossStage = 2;
                        //bossParticles[0].gameObject.SetActive(true);
                        //bossParticles[1].gameObject.SetActive(true);
                        agent.speed = 12;
                        shootTime = 0.15f;
                        Sound.PlaySound("explosion");
                    }
                }
            }
            else if (bossNumber == 10)
            {
                if (bossStage < 2)
                {
                    if (health < (originalhealth / 3 * 2))
                    {
                        bossStage = 2;
                        bossParticles[0].gameObject.SetActive(true);
                        //bossParticles[1].gameObject.SetActive(true);
                        //agent.speed = 12;
                        GetComponent<FinalBoss>().gunFrame.transform.GetChild(0).gameObject.SetActive(true);
                        GetComponent<FinalBoss>().gunFrame.transform.GetChild(2).gameObject.SetActive(true);
                        shootTime = 1f;
                        handler.mainCamera.GetComponent<CameraShake>().Shake(0.05f, 3f);
                        Sound.PlaySound("explosion");
                    }
                }
                else if (bossStage < 3)
                {
                    if (health < (originalhealth / 3))
                    {
                        bossStage = 3;
                        //bossParticles[0].gameObject.SetActive(true);
                        //bossParticles[1].gameObject.SetActive(true);
                        //agent.speed = 12;
                        shootTime = 0.8f;
                        handler.mainCamera.GetComponent<CameraShake>().Shake(0.05f, 3f);
                        Sound.PlaySound("explosion");
                    }
                }
                else if (bossStage == 3)
                {
                    if (bossStage == 3)
                    {
                        if (bossAttackAngle > 0)
                        {
                            bossAttackAngle -= 1;
                        }
                        else
                        {
                            bossAttackAngle = 720;
                        }

                        for (int i = 0; i < 3; i++)
                        {

                            float x = 1 * Mathf.Cos(Mathf.Deg2Rad * (bossAttackAngle / 2));
                            float y = 1 * Mathf.Sin(Mathf.Deg2Rad * (bossAttackAngle / 2));

                            tempplayerpos = new Vector2(transform.position.x + x, transform.position.y + y);
                            GameObject proj = Instantiate(GetComponent<FinalBoss>().bullet5, transform.position, Quaternion.identity);
                            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                            proj.GetComponent<Projectile>().destination = tempplayerpos;
                            proj.GetComponent<Projectile>().ignore = gameObject;

                            //proj.GetComponent<SpriteRenderer>().color = eyeColor;
                        }
                    }
                }
            }
        }

        if (health <= 0)
        {
            int sound = Random.Range(0, 4);

            if (sound == 0)
            {
                Sound.PlaySound("robodeath1");
            }
            else if (sound == 1)
            {
                Sound.PlaySound("robodeath2");
            }
            else if (sound == 2)
            {
                Sound.PlaySound("robodeath3");
            }
            else if (sound == 3)
            {
                Sound.PlaySound("robodeath4");
            }

            if (isBoss)
            {
                int credNo = Random.Range(40, 50);
                int chance = Random.Range(1, 4);

                for (int j = 0; j < chance; j++)
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
                int credNo = Random.Range(1, 4);
                int chance = Random.Range(0, handler.healthChance);

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

            if (handler.currentLevel != 10)
            {
                handler.killed += 1;
            }
            else
            {
                if (isBoss && bossNumber == 10)
                {
                    handler.killed += 1;
                }
            }
            GameObject death = Instantiate(handler.enemyDeath, transform.position, Quaternion.identity);
            death.transform.localScale = transform.localScale;

            death.GetComponent<EnemyDeathColour>().colors = colors;
            death.GetComponent<EnemyDeathColour>().eyeColor = eyeColor;
            death.GetComponent<EnemyDeathColour>().bodyColor = bodyColor;
            death.GetComponent<EnemyDeathColour>().body2Color = body2Color;
            death.GetComponent<EnemyDeathColour>().headColor = headColor;
            death.GetComponent<EnemyDeathColour>().head2Color = head2Color;

            Destroy(gameObject);
        }
        movement = direction;

        if (movement.x > 0.5f)
        {
            movement.x = 1f;
        }
        else if (movement.x < -0.5f)
        {
            movement.x = -1f;
        }
        else
        {
            movement.x = 0f;
        }
        if (movement.y > 0.5f)
        {
            movement.y = 1f;
        }
        else if (movement.y < -0.5f)
        {
            movement.y = -1f;
        }
        else
        {
            movement.y = 0f;
        }

        distance.x = player.transform.position.x - transform.position.x;
        distance.y = player.transform.position.y - transform.position.y;

        if (!handler.inShop && !handler.isPaused)
        {
            if (TwoKeys)
            {
                DiagSwapTimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;;
            }

            if (movement.x == 1 && movement.y == 1)
            {
                TwoKeys = true;
                if ((int)DiagSwapTimer % DiagSwapTime == 0)
                {
                    if (!isShooting)
                    {
                        if (animSwitched)
                        {
                            animator.SetInteger("AnimationNo", 1);
                            animSwitched = false;
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[0];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        }
                        else
                        {
                            animator.SetInteger("AnimationNo", 2);
                            animSwitched = true;
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[1];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 0;

                        }
                    }
                }
            }
            else if (movement.x == 1 && movement.y == -1)
            {
                TwoKeys = true;
                if ((int)DiagSwapTimer % DiagSwapTime == 0)
                {
                    if (!isShooting)
                    {
                        if (animSwitched)
                        {
                            animator.SetInteger("AnimationNo", 2);
                            animSwitched = false;
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[1];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        }
                        else
                        {
                            animator.SetInteger("AnimationNo", 3);
                            animSwitched = true;
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        }
                    }
                }
            }
            else if (movement.x == -1 && movement.y == -1)
            {
                TwoKeys = true;
                if ((int)DiagSwapTimer % DiagSwapTime == 0)
                {
                    if (!isShooting)
                    {
                        if (animSwitched)
                        {
                            animator.SetInteger("AnimationNo", 3);
                            animSwitched = false;
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        }
                        else
                        {
                            animator.SetInteger("AnimationNo", 4);
                            animSwitched = true;
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[3];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        }
                    }
                }
            }
            else if (movement.x == -1 && movement.y == 1)
            {
                TwoKeys = true;
                if ((int)DiagSwapTimer % DiagSwapTime == 0)
                {
                    if (!isShooting)
                    {
                        if (animSwitched)
                        {
                            animator.SetInteger("AnimationNo", 4);
                            animSwitched = false;
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[3];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        }
                        else
                        {
                            animator.SetInteger("AnimationNo", 1);
                            animSwitched = true;
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[0];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        }
                    }
                }
            }
            else
            {
                TwoKeys = false;
            }

            if (isShooting == false)
            {
                if (isRanged)
                {
                    Color tmpcolor = gun.GetComponent<SpriteRenderer>().color;
                    tmpcolor.a = 1f; //Change to 0f to make guns invisible when enemies are disabled/shocked
                    gun.GetComponent<SpriteRenderer>().color = tmpcolor;

                    gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];
                    hand.transform.localPosition = new Vector3(0f, -0.6f);
                    gun.transform.localPosition = new Vector3(0f, -0.344f);

                }

                if (!(movement.x == 0f && movement.y == 0f))
                {
                    //hand.transform.localPosition = new Vector3(movement.x * 0.6f, movement.y * 0.6f);
                }
                if (!TwoKeys)
                {
                    if (movement.x == 0 && movement.y == 0)
                    {
                        animator.SetInteger("AnimationNo", 0);
                        gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    }
                    else
                    {
                        if (movement.x == 1)
                        {
                            animator.SetInteger("AnimationNo", 2);
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[1];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        }
                        else if (movement.x == -1)
                        {
                            animator.SetInteger("AnimationNo", 4);
                            gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[3];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        }
                        else
                        {
                            if (movement.y == 1)
                            {
                                animator.SetInteger("AnimationNo", 1);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[0];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                            }
                            else if (movement.y == -1)
                            {
                                animator.SetInteger("AnimationNo", 3);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            }
                        }
                    }
                }
            }
            else //If you are shooting
            {
                if (isRanged)
                {
                    Color tmpcolor = gun.GetComponent<SpriteRenderer>().color;
                    tmpcolor.a = 1f;
                    gun.GetComponent<SpriteRenderer>().color = tmpcolor;
                }

                Vector3 pos = handler.player.transform.position - transform.position;
                if (pos.x > 0) //If on the right
                {
                    if (pos.y > 0) //Upper right corner
                    {
                        if (pos.x > pos.y) //Right quadrant
                        {
                            hand.transform.localPosition = new Vector3(0.6f, -0.2f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 6);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[1];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                            }
                        }
                        else //Top Quadrant
                        {
                            hand.transform.localPosition = new Vector3(0f, 0.6f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 5);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[0];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                            }
                        }
                        if (TwoKeys)
                        {
                            if ((int)DiagSwapTimer % DiagSwapTime == 0)
                            {
                                if (animSwitched)
                                {
                                    animator.SetInteger("AnimationNo", 6);
                                    animSwitched = false;
                                    gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[1];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                }
                                else
                                {
                                    animator.SetInteger("AnimationNo", 5);
                                    animSwitched = true;
                                    gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[0];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                }
                            }
                        }
                    }
                    else //Bottom right corner
                    {
                        pos.y = -pos.y;
                        if (pos.x > pos.y) //Right quadrant
                        {
                            hand.transform.localPosition = new Vector3(0.6f, -0.2f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 6);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[1];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                            }
                        }
                        else //Bottom Quadrant
                        {
                            hand.transform.localPosition = new Vector3(0f, -0.6f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 7);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            }
                        }
                        if (TwoKeys)
                        {
                            if ((int)DiagSwapTimer % DiagSwapTime == 0)
                            {
                                if (animSwitched)
                                {
                                    animator.SetInteger("AnimationNo", 6);
                                    animSwitched = false;
                                    gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[1];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                }
                                else
                                {
                                    animator.SetInteger("AnimationNo", 7);
                                    animSwitched = true;
                                    gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                                }
                            }
                        }
                    }
                }
                else //If on the left
                {
                    if (pos.y > 0) //Upper left corner
                    {
                        pos.x = -pos.x;
                        if (pos.x > pos.y) //Left quadrant
                        {
                            hand.transform.localPosition = new Vector3(-0.6f, -0.2f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 8);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[3];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                            }
                        }
                        else //Top quadrant
                        {
                            hand.transform.localPosition = new Vector3(0f, 0.6f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 5);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[0];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                            }
                        }
                        if (TwoKeys)
                        {
                            if ((int)DiagSwapTimer % DiagSwapTime == 0)
                            {
                                if (animSwitched)
                                {
                                    animator.SetInteger("AnimationNo", 8);
                                    animSwitched = false;
                                    gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[3];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                }
                                else
                                {
                                    animator.SetInteger("AnimationNo", 5);
                                    animSwitched = true;
                                    gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[0];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                }
                            }
                        }
                    }
                    else //Bottom left corner
                    {
                        if (pos.x < pos.y) //Left quadrant
                        {
                            hand.transform.localPosition = new Vector3(-0.6f, -0.2f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 8);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[3];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                            }
                        }
                        else //Bottom quadrant
                        {
                            hand.transform.localPosition = new Vector3(0f, -0.6f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 7);
                                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            }
                        }
                        if (TwoKeys)
                        {
                            if ((int)DiagSwapTimer % DiagSwapTime == 0)
                            {
                                if (animSwitched)
                                {
                                    animator.SetInteger("AnimationNo", 8);
                                    animSwitched = false;
                                    gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[3];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                }
                                else
                                {
                                    animator.SetInteger("AnimationNo", 7);
                                    animSwitched = true;
                                    gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                                }
                            }
                        }
                    }
                }
            }
        }
        /*if (isShooting == false) //For making hand look in all 8 directions
        {
            hand.transform.localPosition = new Vector3(movement.x / 100, movement.y / 100);
        }
        else
        {
        //Quadrant Code
        }
        */
        /*
        Vector3 pos = player.transform.position - transform.position;
        if (pos.x > 0) //If on the right
        {
            if (pos.y > 0) //Upper right corner
            {
                if (pos.x > pos.y) //Right quadrant
                {
                    hand.transform.localPosition = new Vector3(0.01f, 0f);
                }
                else //Top Quadrant
                {
                    hand.transform.localPosition = new Vector3(0f, 0.01f);
                }
            }
            else //Bottom right corner
            {
                pos.y = -pos.y;
                if (pos.x > pos.y) //Right quadrant
                {
                    hand.transform.localPosition = new Vector3(0.01f, 0f);
                }
                else //Bottom Quadrant
                {
                    hand.transform.localPosition = new Vector3(0f, -0.01f);
                }
            }
        }
        else //If on the left
        {
            if (pos.y > 0) //Upper left corner
            {
                pos.x = -pos.x;
                if (pos.x > pos.y) //Left quadrant
                {
                    hand.transform.localPosition = new Vector3(-0.01f, 0f);
                }
                else //Top quadrant
                {
                    hand.transform.localPosition = new Vector3(0f, 0.01f);
                }
            }
            else //Bottom left corner
            {
                if (pos.x < pos.y) //Left quadrant
                {
                    hand.transform.localPosition = new Vector3(-0.01f, 0f);
                }
                else //Bottom quadrant
                {
                    hand.transform.localPosition = new Vector3(0f, -0.01f);
                }
            }
        }
        */

        if (isBoss && bossNumber == 8 && bossWokenUp && (moveDisabled == false || (pausedForAttack && !stunned)))
        {
            
            if (health > (originalhealth / 2))
            {
                eyeColor.b = (health - (originalhealth / 2)) / (originalhealth / 2);
            }
            else
            {
                bossParticles[0].gameObject.SetActive(true);

                eyeColor.b = 0f;
                eyeColor.g = health / (originalhealth / 2);

                bossParticles[0].startColor = eyeColor;

            }

            originalEyeColor = eyeColor;
            gun.GetComponent<SpriteRenderer>().color = eyeColor;
            handler.bossHB.GetComponent<BossHealthBar>().ResetColor();
        }
        else if (isBoss && bossNumber == 9 && bossWokenUp && (moveDisabled == false || (pausedForAttack && !stunned)))
        {
            if (rainbowUp)
            {
                rainbowHue += 0.002f * 60f / (1f / Time.deltaTime); //Change first number for rate;;
            }
            else
            {
                rainbowHue -= 0.002f * 60f / (1f / Time.deltaTime); //Change first number for rate;;
            }

            if (rainbowHue > 1f)
            {
                rainbowUp = false;
                rainbowHue = 1f;
            }
            else if (rainbowHue < 0f)
            {
                rainbowUp = true;
                rainbowHue = 0f;
            }

            eyeColor = Color.HSVToRGB(rainbowHue, 1f, 1f);

            if (health > (originalhealth / 2))
            {
                ;
            }
            else
            {
                bossParticles[0].gameObject.SetActive(true);

                bossParticles[0].startColor = eyeColor;

            }

            originalEyeColor = eyeColor;
            gun.GetComponent<SpriteRenderer>().color = eyeColor;
            handler.bossHB.GetComponent<BossHealthBar>().ResetColor();
        }

        shootTimer -= Time.deltaTime;
        if (shootTimer < 0)
        {
            if (moveDisabled == false || (pausedForAttack && !stunned))
            {
                if (inRange)
                {
                    if (!pausedForAttack)
                    {
                        isShooting = true;
                    }

                    if (isRanged)
                    {

                        if (!isBoss || bossNumber == 0 || bossNumber == 1 || bossNumber == 2 || bossNumber == 3)
                        {
                            for (int i = 0; i < bulletCount; i++)
                            {
                                tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                proj.GetComponent<Projectile>().destination = tempplayerpos;
                                proj.GetComponent<Projectile>().ignore = gameObject;
                                if (!isBoss)
                                {
                                    proj.GetComponent<Projectile>().damage = damage;
                                }

                                proj.GetComponent<SpriteRenderer>().color = eyeColor;
                            }

                            shootTimer = shootTime;
                        }
                        else
                        {
                            if (bossNumber == 4)
                            {
                                if (attackStack <= 0)
                                {
                                    attackType = Random.Range(1, 101);
                                    pausedForAttack = false;
                                }
                                else
                                {
                                    pausedForAttack = true;
                                }

                                if (attackType <= 80 && bannedAttack != 1)
                                {
                                    currentAttack = 1;

                                    for (int i = 0; i < bulletCount; i++)
                                    {
                                        tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                        GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                    }

                                    shootTimer = shootTime;
                                }
                                else if (attackType <= 100 && bannedAttack != 2)
                                {

                                    if (attackStack > 2 && currentAttack == 2)
                                    {
                                        if (bossStage == 1)
                                        {
                                            for (int i = -1; i < 2; i++)
                                            {
                                                for (int j = -1; j < 2; j++)
                                                {
                                                    if ((i != 0 || j != 0) && (i == 0 || j == 0))
                                                    {
                                                        tempplayerpos = new Vector2(transform.position.x + i, transform.position.y + j);
                                                        GameObject proj = Instantiate(bullet, transform.position, Quaternion.identity);
                                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int i = -1; i < 2; i++)
                                            {
                                                for (int j = -1; j < 2; j++)
                                                {
                                                    if (i != 0 || j != 0)
                                                    {
                                                        tempplayerpos = new Vector2(transform.position.x + i, transform.position.y + j);
                                                        GameObject proj = Instantiate(bullet, transform.position, Quaternion.identity);
                                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    pausedForAttack = true;
                                    currentAttack = 2;

                                    shootTimer = shootTime;

                                    if (attackStack <= 0)
                                    {
                                        attackStack = 6; //3 for waiting, rest for attacking
                                    }
                                }
                            }
                            else if (bossNumber == 6)
                            {
                                if (attackStack <= 0)
                                {
                                    attackType = Random.Range(1, 101);
                                    pausedForAttack = false;
                                }
                                else
                                {
                                    //pausedForAttack = true;
                                }

                                if (attackType <= 75 && bannedAttack != 1)
                                {
                                    currentAttack = 1;

                                    for (int i = 0; i < bulletCount; i++)
                                    {
                                        tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                        GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                    }

                                    shootTimer = shootTime;
                                }
                                else if (attackType <= 80 && bannedAttack != 3 && bossStage == 2)
                                {
                                    if (attackStack > 2 && currentAttack == 3)
                                    {
                                        for (int i = 0; i < bulletCount; i++)
                                        {
                                            tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                            GameObject proj = Instantiate(tertiaryBullet, hand.transform.position, Quaternion.identity);
                                            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                            proj.GetComponent<Projectile>().destination = tempplayerpos;
                                            proj.GetComponent<Projectile>().ignore = gameObject;

                                            proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                        }
                                    }

                                    //pausedForAttack = true;
                                    currentAttack = 3;

                                    shootTimer = shootTime / 3;

                                    if (attackStack <= 0)
                                    {
                                        attackStack = 23; //3 for waiting, rest for attacking
                                    }
                                }
                                else if (attackType <= 85 && bannedAttack != 4)
                                {
                                    if (attackStack > 2 && currentAttack == 4)
                                    {
                                        for (int i = -1; i < 2; i++)
                                        {
                                            for (int j = -1; j < 2; j++)
                                            {
                                                if (i != 0 || j != 0)
                                                {
                                                    tempplayerpos = new Vector2(transform.position.x + i, transform.position.y + j);
                                                    GameObject proj = Instantiate(tertiaryBullet, transform.position, Quaternion.identity);
                                                    tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                                    proj.GetComponent<Projectile>().destination = tempplayerpos;
                                                    proj.GetComponent<Projectile>().ignore = gameObject;

                                                    proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                                }
                                            }
                                        }
                                    }

                                    pausedForAttack = true;
                                    currentAttack = 4;

                                    shootTimer = shootTime / 2;

                                    if (attackStack <= 0)
                                    {
                                        attackStack = 8; //3 for waiting, rest for attacking
                                    }
                                }

                                else if (attackType <= 100 && bannedAttack != 2)
                                {
                                    if (attackStack > 2 && currentAttack == 2)
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
                                            GameObject proj = Instantiate(secondaryBullet, hand.transform.position, Quaternion.identity);
                                            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                            proj.GetComponent<Projectile>().destination = tempplayerpos;
                                            proj.GetComponent<Projectile>().ignore = gameObject;

                                            proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                        }
                                    }

                                    pausedForAttack = true;
                                    currentAttack = 2;

                                    shootTimer = shootTime;

                                    if (attackStack <= 0)
                                    {
                                        attackStack = 6; //3 for waiting, rest for attacking
                                    }
                                }
                            }
                            else if (bossNumber == 7) //Nuclear Boss
                            {
                                if (attackStack <= 0)
                                {
                                    attackType = Random.Range(1, 101);
                                    pausedForAttack = false;
                                }
                                else
                                {
                                    //pausedForAttack = true;
                                }

                                if (attackType <= 40 && bannedAttack != 1)
                                {
                                    currentAttack = 1;

                                    for (int i = 0; i < bulletCount; i++)
                                    {
                                        tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                        GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                    }

                                    shootTimer = shootTime;

                                    if (attackStack <= 0)
                                    {
                                        attackStack = Random.Range(8, 23); //3 for waiting, rest for attacking
                                    }
                                }
                                else if (attackType > 40 && attackType <= 70 && bannedAttack != 2)
                                {
                                    if (attackStack > 2 && currentAttack == 2)
                                    {
                                        if (bossAttackAngle < 360)
                                        {
                                            if (bossStage == 2)
                                            {
                                                bossAttackAngle += 2;
                                            }
                                            else
                                            {
                                                bossAttackAngle += 9;
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
                                            GameObject proj = Instantiate(bullet, transform.position, Quaternion.identity);
                                            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                            proj.GetComponent<Projectile>().destination = tempplayerpos;
                                            proj.GetComponent<Projectile>().ignore = gameObject;

                                            proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                        }
                                    }

                                    pausedForAttack = true;
                                    currentAttack = 2;

                                    if (bossStage == 2)
                                    {
                                        shootTimer = shootTime / 20;
                                    }
                                    else
                                    {
                                        shootTimer = shootTime / 2;
                                    }

                                    if (attackStack <= 0)
                                    {
                                        if (bossStage == 2)
                                        {
                                            attackStack = 183; //3 for waiting, rest for attacking
                                        }
                                        else
                                        {
                                            attackStack = 43; //3 for waiting, rest for attacking
                                        }
                                    }
                                }
                                else if (attackType > 70 && attackType <= 100 && bannedAttack != 3)
                                {
                                    if (attackStack > 2 && currentAttack == 3)
                                    {
                                        int angle = Random.Range(0, 361);

                                        float x = 1 * Mathf.Cos(Mathf.Deg2Rad * angle);
                                        float y = 1 * Mathf.Sin(Mathf.Deg2Rad * angle);

                                        tempplayerpos = new Vector2(transform.position.x + x, transform.position.y + y);
                                        GameObject proj = Instantiate(secondaryBullet, transform.position, Quaternion.identity);
                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                        proj.GetComponent<Projectile>().homingTarget = handler.player;
                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                    }

                                    //pausedForAttack = true;
                                    currentAttack = 3;

                                    shootTimer = shootTime * 2;

                                    if (attackStack <= 0)
                                    {
                                        if (bossStage == 2)
                                        {
                                            attackStack = 8; //3 for waiting, rest for attacking
                                        }
                                        else
                                        {
                                            attackStack = 6; //3 for waiting, rest for attacking
                                        }
                                    }
                                }
                            }
                            else if (bossNumber == 8) //Mini Boss
                            {
                                if (attackStack <= 0)
                                {
                                    attackType = Random.Range(1, 101);
                                    pausedForAttack = false;
                                }
                                else
                                {
                                    //pausedForAttack = true;
                                }

                                if (attackType <= 40 && bannedAttack != 1)
                                {
                                    currentAttack = 1;

                                    for (int i = 0; i < bulletCount; i++)
                                    {
                                        tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                        GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                    }

                                    shootTimer = shootTime;

                                    if (attackStack <= 0)
                                    {
                                        attackStack = Random.Range(8, 23); //3 for waiting, rest for attacking
                                    }
                                }
                                else if (attackType > 40 && attackType <= 70 && bannedAttack != 2)
                                {
                                    if (attackStack > 2 && currentAttack == 2)
                                    {

                                        for (int i = 0; i < 91; i++)
                                        {

                                            float x = 1 * Mathf.Cos(Mathf.Deg2Rad * (i * 4));
                                            float y = 1 * Mathf.Sin(Mathf.Deg2Rad * (i * 4));

                                            tempplayerpos = new Vector2(transform.position.x + x, transform.position.y + y);
                                            GameObject proj = Instantiate(bullet, transform.position, Quaternion.identity);
                                            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                            proj.GetComponent<Projectile>().destination = tempplayerpos;
                                            proj.GetComponent<Projectile>().ignore = gameObject;

                                            proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                        }
                                    }

                                    pausedForAttack = true;
                                    currentAttack = 2;

                                    if (bossStage == 2)
                                    {
                                        shootTimer = shootTime * 3;
                                    }
                                    else
                                    {
                                        shootTimer = shootTime * 5;
                                    }

                                    if (attackStack <= 0)
                                    {
                                        if (bossStage == 2)
                                        {
                                            attackStack = 8; //3 for waiting, rest for attacking
                                        }
                                        else
                                        {
                                            attackStack = 6; //3 for waiting, rest for attacking
                                        }
                                    }
                                }
                                else if (attackType > 70 && attackType <= 100 && bannedAttack != 3)
                                {
                                    if (attackStack > 2 && currentAttack == 3)
                                    {
                                        tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                        GameObject proj = Instantiate(secondaryBullet, hand.transform.position, Quaternion.identity);
                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                    }

                                    pausedForAttack = true;
                                    currentAttack = 3;

                                    shootTimer = shootTime * 3;

                                    if (attackStack <= 0)
                                    {
                                        if (bossStage == 2)
                                        {
                                            attackStack = 6; //3 for waiting, rest for attacking
                                        }
                                        else
                                        {
                                            attackStack = 4; //3 for waiting, rest for attacking
                                        }
                                    }
                                }
                            }
                            else if (bossNumber == 9) //Destruction Boss
                            {
                                if (attackStack <= 0)
                                {
                                    attackType = Random.Range(1, 101);
                                    pausedForAttack = false;
                                }
                                else
                                {
                                    //pausedForAttack = true;
                                }

                                if (attackType <= 100 && bannedAttack != 1)
                                {
                                    currentAttack = 1;

                                    int bulletType = Random.Range(1, 101);

                                    if (bulletType < 20)
                                    {
                                        int offsetX = Random.Range(-1000, 1000);
                                        int offsetY = Random.Range(-1000, 1000);

                                        while (offsetX == 0 && offsetY == 0)
                                        {
                                            Debug.Log("BOTH WERE ZERO");
                                            offsetX = Random.Range(-1000, 1000);
                                            offsetY = Random.Range(-1000, 1000);
                                        }

                                        tempplayerpos = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
                                        GameObject proj = Instantiate(tertiaryBullet, hand.transform.position, Quaternion.identity);
                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                    }
                                    else if (bulletType < 30)
                                    {
                                        tempplayerpos = new Vector2(transform.position.x, transform.position.y);
                                        GameObject proj = Instantiate(secondaryBullet, transform.position, Quaternion.identity);
                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                        proj.GetComponent<Projectile>().homingTarget = handler.player;
                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                        pausedForAttack = true;
                                        attackStack = 6;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < bulletCount; i++)
                                        {
                                            tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                            GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                            proj.GetComponent<Projectile>().destination = tempplayerpos;
                                            proj.GetComponent<Projectile>().ignore = gameObject;

                                            proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                        }
                                    }

                                    shootTimer = shootTime;
                                }
                            }
                            else if (bossNumber == 10) //Ultimate Boss
                            {

                                if (attackStack <= 0)
                                {
                                    bannedAttack = currentAttack;
                                    attackType = Random.Range(1, 101);
                                    pausedForAttack = true;
                                    Debug.Log("CHANGED");
                                }
                                else
                                {
                                    //pausedForAttack = true;
                                }

                                GetComponent<FinalBoss>().Attack(attackType, bannedAttack);
                                currentAttack = GetComponent<FinalBoss>().currentAttack;

                                Debug.Log("TYPE: " + attackType);
                                Debug.Log("ATTACK: " + currentAttack);
                                Debug.Log("Current STACK:" + attackStack);
                            }

                            if (attackStack > 0)
                            {
                                attackStack -= 1;
                            }
                        }
                    }
                    else
                    {
                        if (touchingPlayer && !isBoss)
                        {
                            for (int i = 0; i < bulletCount; i++)
                            {
                                tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                proj.GetComponent<Projectile>().destination = tempplayerpos;
                                proj.GetComponent<Projectile>().ignore = gameObject;

                                if (!isBoss)
                                {
                                    proj.GetComponent<Projectile>().damage = damage;
                                }

                                proj.GetComponent<SpriteRenderer>().color = eyeColor;
                            }

                            shootTimer = shootTime;
                        }
                        else
                        {
                            if (isBoss)
                            {
                                if (bossNumber == 2)
                                {
                                    if (attackStack <= 0)
                                    {
                                        attackType = Random.Range(1, 101);
                                        pausedForAttack = false;
                                    }
                                    else
                                    {
                                        pausedForAttack = true;
                                    }

                                    if (bossStage == 2 && attackType <= 10 && bannedAttack != 3)
                                    {
                                        if (attackStack > 1 && currentAttack == 3) //attackStack > x; x + 1 = number of waiting attacks
                                        {
                                            for (int i = 0; i < bulletCount; i++)
                                            {
                                                tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                                GameObject proj = Instantiate(tertiaryBullet, hand.transform.position, Quaternion.identity);
                                                tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                                proj.GetComponent<Projectile>().destination = tempplayerpos;
                                                proj.GetComponent<Projectile>().ignore = gameObject;

                                                proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                            }
                                        }

                                        pausedForAttack = true;
                                        currentAttack = 3;

                                        shootTimer = shootTime;

                                        if (attackStack <= 0)
                                        {
                                            attackStack = 5; //2 for waiting, rest for attacking
                                        }
                                    }
                                    else if (attackType <= 95 && bannedAttack != 1)
                                    {
                                        currentAttack = 1;

                                        if (touchingPlayer)
                                        {
                                            for (int i = 0; i < bulletCount; i++)
                                            {
                                                tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                                GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                                tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                                proj.GetComponent<Projectile>().destination = tempplayerpos;
                                                proj.GetComponent<Projectile>().ignore = gameObject;

                                                proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                            }

                                            shootTimer = shootTime;
                                        }
                                    }
                                    else if (attackType <= 100 && bannedAttack != 2)
                                    {

                                        if (attackStack > 1 && currentAttack == 2) //attackStack > x; x + 1 = number of waiting attacks
                                        {
                                            for (int i = 0; i < bulletCount; i++)
                                            {
                                                tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                                GameObject proj = Instantiate(secondaryBullet, hand.transform.position, Quaternion.identity);
                                                tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                                proj.GetComponent<Projectile>().destination = tempplayerpos;
                                                proj.GetComponent<Projectile>().ignore = gameObject;

                                                proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                            }
                                        }

                                        pausedForAttack = true;
                                        currentAttack = 2;

                                        shootTimer = shootTime / 2;

                                        if (attackStack <= 0)
                                        {
                                            attackStack = 7; //2 for waiting, rest for attacking
                                        }
                                    }
                                }

                                if (bannedCounter > 0)
                                {
                                    bannedCounter -= 1;
                                }
                                else
                                {
                                    bannedAttack = 0;
                                }

                                if (attackStack > 0)
                                {
                                    attackStack -= 1;

                                    if (attackStack <= 0)
                                    {
                                        shootTimer = shootTime * 1.5f;
                                        bannedAttack = currentAttack;
                                        bannedCounter = 200;
                                    }
                                }
                            }
                        }
                        /*
                        attackType = Random.Range(1, 101);

                                if (attackType == 1)
                                {
                                    if (touchingPlayer)
                                    {
                                        for (int i = 0; i < bulletCount; i++)
                                        {
                                            tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                            GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                            tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                            proj.GetComponent<Projectile>().destination = tempplayerpos;
                                            proj.GetComponent<Projectile>().ignore = gameObject;

                                            proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                        }

                                        shootTimer = shootTime;
                                    }
                                }
                                else if (attackType > 2)
                                {
                                    for (int i = 0; i < bulletCount; i++)
                                    {
                                        tempplayerpos = new Vector2(player.transform.position.x, player.transform.position.y);
                                        GameObject proj = Instantiate(bullet, hand.transform.position, Quaternion.identity);
                                        tempplayerpos = tempplayerpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                        proj.GetComponent<Projectile>().destination = tempplayerpos;
                                        proj.GetComponent<Projectile>().ignore = gameObject;

                                        proj.GetComponent<SpriteRenderer>().color = eyeColor;
                                    }

                                    pausedForAttack = true;
                                    currentAttack = 2;

                                    shootTimer = shootTime;

                                    if (attackStack <= 0)
                                    {
                                        attackStack = 6; //3 for waiting, rest for attacking
                                    }
                                }

                            }
                        }
                        */
                    }
                }
            }
        }
        if (!moveDisabled)
        {
            if (!inRange)
            {
                isShooting = false;
            }
            else
            {
                if (!stunned && !pausedForAttack)
                {
                    isShooting = true;
                }
            }
        }
        else
        {
            if (stunned)
            {
                gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];
                hand.transform.localPosition = new Vector3(0f, -0.6f);
                gun.transform.localPosition = new Vector3(0f, -0.344f);
            }
        }
    }
    public void SetEnemyX(float hfx)
    {
        gun.transform.localPosition = new Vector3(hfx, gun.transform.localPosition.y);
    }
    public void SetEnemyY(float hfy)
    {
        gun.transform.localPosition = new Vector3(gun.transform.localPosition.x, hfy);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Stun(0.75f, 2);
        }
    }
    public void Stun(float duration, int type) //Why is there 2 types?
    {
        if (!stunned)
        {
            Invoke("EndStun", duration);

            eyeColor = Color.black;

            if (!isRanged)
            {

                gun.GetComponent<SpriteRenderer>().sortingOrder = 2;
                gun.GetComponent<SpriteRenderer>().sprite = allEnemyGuns[2];
                hand.transform.localPosition = new Vector3(0f, -0.6f);
                gun.transform.localPosition = new Vector3(0f, -0.344f);
            }

            stunned = true;

            moveDisabled = true;
            isShooting = false;

            if (type == 1 || type == 2)
            {
                bool hasGlow = false;

                foreach (Transform child in transform)
                {
                    if (child.gameObject.name == "DashGlow(Clone)")
                    {
                        hasGlow = true;
                    }
                }

                if (!hasGlow)
                {
                    int dashParticleType = Random.Range(1, 4);

                    GameObject particle = Instantiate(handler.dashParticles, transform.position, Quaternion.identity);
                    particle.GetComponent<Animator>().SetInteger("Number", dashParticleType);
                    particle.transform.localScale = transform.localScale;
                    particle.transform.SetParent(transform);

                    GameObject glow = Instantiate(handler.dashGlow, transform.position, Quaternion.identity);
                    glow.transform.localScale = transform.localScale;
                    glow.transform.SetParent(transform);
                }
            }
        }
    }
    void EndStun()
    {
        stunned = false;

        eyeColor = originalEyeColor;

        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "DashGlow(Clone)" || child.gameObject.name == "DashParticles(Clone)")
            {
                Destroy(child.gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "MeleeSlash(Clone)")
        {
            if (!isInvicible)
            {
                float dmg2;

                if (collision.GetComponent<Projectile>().isCritical)
                {
                    handler.crosshairState = 2;
                    float dmg = originalhealth * handler.meleeDamage[handler.currentLevel - 1] * 3;
                    health -= dmg;

                    dmg2 = dmg;
                }
                else
                {
                    handler.crosshairState = 1;
                    float dmg = originalhealth * handler.meleeDamage[handler.currentLevel - 1];
                    health -= dmg;

                    dmg2 = dmg;
                }

                GameObject popup = Instantiate(handler.dmgPopUp, transform.position, Quaternion.identity);
                popup.GetComponent<TextPopUp>().text = ((int)dmg2).ToString();
                popup.GetComponent<TextPopUp>().target = gameObject;
                popup.GetComponent<TextPopUp>().damage = (int)dmg2;

                if (collision.GetComponent<Projectile>().isCritical)
                {
                    popup.GetComponent<TextPopUp>().crit = true;
                }
                else
                {
                    handler.crosshairState = 1;
                }
            }
        }
        else if (collision.gameObject.name == "WaveBullet(Clone)")
        {
            if (!isInvicible)
            {
                health -= collision.GetComponent<Projectile>().damage;

                GameObject popup = Instantiate(handler.dmgPopUp, transform.position, Quaternion.identity);
                popup.GetComponent<TextPopUp>().text = ((int)collision.GetComponent<Projectile>().damage).ToString();
                popup.GetComponent<TextPopUp>().target = gameObject;
                popup.GetComponent<TextPopUp>().damage = (int)collision.GetComponent<Projectile>().damage;

                if (collision.GetComponent<Projectile>().isCritical)
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
        else if (collision.gameObject.name == "HCBullet(Clone)")
        {
            if (!isInvicible)
            {
                health -= collision.GetComponent<Projectile>().damage;

                GameObject popup = Instantiate(handler.dmgPopUp, transform.position, Quaternion.identity);
                popup.GetComponent<TextPopUp>().text = ((int)collision.GetComponent<Projectile>().damage).ToString();
                popup.GetComponent<TextPopUp>().target = gameObject;
                popup.GetComponent<TextPopUp>().damage = (int)collision.GetComponent<Projectile>().damage;

                if (collision.GetComponent<Projectile>().isCritical)
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
        else if (collision.gameObject.name == "Explosion(Clone)")
        {
            if (!isInvicible)
            {
                health -= collision.GetComponent<Explosion>().damage;

                GameObject popup = Instantiate(handler.dmgPopUp, transform.position, Quaternion.identity);
                popup.GetComponent<TextPopUp>().text = ((int)collision.GetComponent<Explosion>().damage).ToString();
                popup.GetComponent<TextPopUp>().target = gameObject;
                popup.GetComponent<TextPopUp>().damage = (int)collision.GetComponent<Explosion>().damage;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //touchingPlayer = true;
        }
        if (destroyer)
        {
            if (collision.gameObject.tag == "Wall" && collision.gameObject.GetComponent<PolygonCollider2D>() != null)
            {
                Sound.PlaySound("explosion");
                Instantiate(handler.spirePE, collision.gameObject.transform.position, Quaternion.identity);
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.tag == "Crate")
            {
                collision.gameObject.GetComponent<Crate>().health = 0;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //touchingPlayer = false;
        }
    }

    private void BeginBossWakeUp()
    {
        InvokeRepeating("BossWakeUp", 0f, 0.01f);
    }

    private void BossWakeUp()
    {
        isInvicible = true;

        permaDisabled = true;

        bossWakingUp = true;

        float r = originalEyeColor.r / 400;
        float g = originalEyeColor.g / 400;
        float b = originalEyeColor.b / 400;

        Color x = new Color(eyeColor.r + r, eyeColor.g + g, eyeColor.b + b);

        if (x.r > originalEyeColor.r)
        {
            x.r = originalEyeColor.r;
        }
        if (x.g > originalEyeColor.g)
        {
            x.g = originalEyeColor.g;
        }
        if (x.b > originalEyeColor.b)
        {
            x.b = originalEyeColor.b;
        }

        eyeColor = x;

        if (x == originalEyeColor)
        {
            handler.player.GetComponent<PlayerMove>().followcam.GetComponent<FollowCamera>().FinishBossMove(gameObject);
            CancelInvoke("BossWakeUp");
        }
    }
    public void Undisable()
    {
        isInvicible = false;
        permaDisabled = false;
        bossWakingUp = false;
        bossWokenUp = true;
    }

    //Functions for final boss to be used in conjuction with FinalBoss script
    public void SetShootTime(float time)
    {
        shootTimer = time;
    }

    public void SetAttackStack(int stack)
    {
        attackStack = stack;
        Debug.Log("STACK: " + attackStack);
    }
    public int GetAttackStack()
    {
        return attackStack;
    }

    public void BounceGuns(float amount)
    {
        if (isBoss && bossNumber == 10)
        {
            GetComponent<FinalBoss>().BounceBossGuns(amount);
        }
    }
}