using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering.Universal;

//Remove settings from game scene
//Check all settings

//To Do:

//Make teleporters interactable and not by collision

//Make separate health drop chance

//Can Do:
//Particles + Camera Shake

//Might Do:
//Chapter Text pop up
//New enemy weapons

//Soul takes over enemy for a bit

//Bugs
//Fix idle animation glitch
//Clothing position glitch on right + down hold?
//Brother glitches on teleport?

//Fix fader glitch when on level select

public class GameHandler : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject player;
    public GameObject settings;
    public GameObject postprocessing;

    public TextMeshProUGUI volumeText;
    public Slider volumeSlider;

    public float health;
    public float maxHealth;
    public float critChance;

    public GameObject EEHandler;

    public GameObject dashObj;
    public GameObject warpObj;
    public GameObject healthbar;
    public GameObject losthealthbar;

    public GameObject emp;

    public GameObject enemyMarker;
    private GameObject nearestEnemy;

    private float lostHealthFillAmount; //To check whether CatchUp has been called yet
    private bool finishedLostCoroutine; //Check whether first CatchUp in queue is finished

    public int money;
    public TextMeshProUGUI credCounter;
    public TextMeshProUGUI shopcredCounter;
    public TextMeshProUGUI killedCounter;

    public GameObject credprefab;
    public GameObject hpprefab;

    public GameObject ohBar;
    public GameObject ultBar;
    public GameObject dashBar;

    public GameObject ohParticles;
    public GameObject ultParticles;

    public GameObject UltimateUI;

    public TextMeshProUGUI equippedText;

    private string[] gunEquippedTexts = new string[]{"Pistol", "Minigun", "Shotgun", "Slice Rifle", "Hand Cannon", "Ion Launcher", "Katana"};

    public GameObject enemies;

    public float ohValue;
    public bool overheated;

    public float ultValue;
    public bool ultReady;
    public bool ultActive;

    public bool ultPaused;

    public float dashValue;

    public int healthChance;

    public GameObject faderobj;

    public Vector3 mousePosition;
    public GameObject cursor;
    public int screen_bound;

    public GameObject mainui;
    public GameObject pausemenu;
    public bool isPaused;
    public GameObject shopmenu;
    public bool inShop;

    public bool shopInRange;
    public GameObject shopTexts;

    public GameObject overlayUI;

    public int colourCost;

    public int currentLevel;

    public int[] enemyNo; //Number of enemies in a level

    public int killed; //Number of enemies killed

    public float pbound; //How far enemies must be to be out of range

    public Sprite[] Telepads;

    public Sprite brokenCrate;
    public Vector3[] levelPos;

    public bool ultTransition;

    //Shop UI
    public Slider HRed;
    public Slider HGreen;
    public Slider HBlue;
    public Slider CRed;
    public Slider CGreen;
    public Slider CBlue;

    public Vector3 hairRGB;
    public Vector3 clothesRGB;

    public GameObject shopHair;
    public GameObject shopClothes;

    public Button buyHealthButton;
    public TextMeshProUGUI healthPriceText;
    public TextMeshProUGUI healthBuyText;
    public TextMeshProUGUI healthLevelText;
    public int[] healthPrices;
    public int healthLevel;

    public TextMeshProUGUI[] gunPriceTexts;
    public int[] gunPrices;
    public bool[] gunBought;
    public Button[] buyGuns;
    public Button applyColor;

    public int gunEquipped;
    public float[] gunFirerates;
    public float[] ohIncrements;
    public float[] gunSpread;
    public float[] barrelPosX;
    public float[] barrelPosY;
    public GameObject[] bullets;

    public int tempGun;

    public bool FadePauseMenu;

    public Sprite[] guns1; //Starter Pistol
    public Sprite[] guns2; //Minigun
    public Sprite[] guns3; //Shotgun
    public Sprite[] guns4; //Slice Rifle
    public Sprite[] guns5; //Hand Cannon
    public Sprite[] guns6; //RPG
    public Sprite[] guns7; //Katana

    public Sprite[] guns8; //Enemy Gun
    public Sprite[] guns9; //Enemy Minigun
    public Sprite[] guns10; //Enemy Shotgun
    public Sprite[] guns11; //Enemy Slice Rifle
    public Sprite[] guns12; //Enemy Hand Cannon
    public Sprite[] guns13; //Enemy RPG
    public Sprite[] guns14; //Enemy Katana

    public Sprite[] goldKatana;

    public List<Sprite[]> enemyGuns = new List<Sprite[]>();

    public float[] meleeDamage;
    //(0.15f, 0.10f, 0.5f, 0.2f, 0.005f)

    public List<Sprite[]> allGuns = new List<Sprite[]>();

    public ParticleSystem ultPE;
    public ParticleSystem BultPe;
    public ParticleSystem flashPE;
    public ParticleSystem debris1;
    public ParticleSystem slashPE;
    public ParticleSystem sparkPE;
    public ParticleSystem spirePE;

    public GameObject explosion;
    public GameObject enemyExplosion;
    public GameObject poisonPool;
    public GameObject firePool;
    public GameObject clusterBullet;

    private bool hidBrother;

    private float musicTimer;
    private bool deathPlayed;

    public float playerHitTimer;
    private Color vignettecolor;

    public bool gameFinished;

    private float gameTimer;

    public GameObject dmgPopUp;

    public int crosshairResetTime;
    private float crosshairTimer;
    public int crosshairState;

    [SerializeField] private Texture2D[] crosshairImages;

    public GameObject hitIndicator;
    public GameObject enemyDeath;
    public GameObject screenCrack;

    public GameObject dashParticles;
    public GameObject dashGlow;

    public GameObject coneSparks;
    public GameObject ShottySparks;

    public GameObject tpParticles;

    public GameObject enemyHealthBar;

    public GameObject bulletHole;

    public TextMeshProUGUI fpsCounter;

    public int[] robotHealth;
    public int[] robotDamage;

    public int[] meleeBotHealth;
    public int[] meleeBotDamage;

    public GameObject[] bosses;
    public GameObject bossHB;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.gameObject;

        gameTimer = 0;
        gameFinished = false;
        isPaused = false;
        inShop = false;
        pausemenu.SetActive(false);
        shopmenu.SetActive(false);
        shopInRange = false;

        faderobj.GetComponent<Fade>().FaderOut(2f, 2f);

        deathPlayed = false;
        playerHitTimer = 100;

        //Camera.main.aspect = 16f / 9f;

        //currentLevel = 1; /////
        settings = GameObject.Find("Settings");

        volumeSlider.value = settings.GetComponent<Settings>().volume;
        AudioListener.volume = settings.GetComponent<Settings>().volume;

        currentLevel = settings.GetComponent<Settings>().currentLevel;
        gunEquipped = settings.GetComponent<Settings>().currentGun;
        gunBought = settings.GetComponent<Settings>().gunBought;
        hairRGB = settings.GetComponent<Settings>().hairRGB;
        clothesRGB = settings.GetComponent<Settings>().clothesRGB;

        tempGun = gunEquipped;
        healthLevel = settings.GetComponent<Settings>().healthLevel;

        if (settings.GetComponent<Settings>().foundKatana)
        {
            guns7 = goldKatana;
        }

        float temphealth = health;
        for (int i = 0; i < healthLevel; i++)
        {
            temphealth = temphealth * 1.2f;
        }

        maxHealth = temphealth;
        health = maxHealth;

        money = settings.GetComponent<Settings>().cells;
        ohValue = 0;
        overheated = false;

        ultValue = 0;
        ultReady = false;
        ultTransition = false;


        FadePauseMenu = false;

        if (settings.GetComponent<Settings>().foundCrown)
        {
            player.GetComponent<PlayerMove>().brother.GetComponent<SoulFollow>().ActivateCrown();
        }
        
        for (int i = 0; i < 10; i++) //Change to Level Number
        {
            enemyNo[i] = enemies.transform.GetChild(i).childCount;

            /*
            for (int j = 0; j < enemies.transform.GetChild(i).childCount; j++) //Makes all robot's hands (1 red pixel) invisible
            {
                Color x = enemies.transform.GetChild(i).transform.GetChild(j).GetChild(0).GetComponent<SpriteRenderer>().color;
                x.a = 0f;
                enemies.transform.GetChild(i).transform.GetChild(j).GetChild(0).GetComponent<SpriteRenderer>().color = x;
            }
            */
        }
        

        allGuns.Add(guns1);
        allGuns.Add(guns2);
        allGuns.Add(guns3);
        allGuns.Add(guns4);
        allGuns.Add(guns5);
        allGuns.Add(guns6);
        allGuns.Add(guns7);

        enemyGuns.Add(guns8);
        enemyGuns.Add(guns9);
        enemyGuns.Add(guns10);
        enemyGuns.Add(guns11);
        enemyGuns.Add(guns12);
        enemyGuns.Add(guns13);
        enemyGuns.Add(guns14);

        hidBrother = false;

        vignettecolor = new Color(0f, 0.5f, 1f);

        Cursor.SetCursor(crosshairImages[0], new Vector2(crosshairImages[crosshairState].width / 2, crosshairImages[crosshairState].height / 2), CursorMode.ForceSoftware);
        crosshairTimer = 0;

        losthealthbar.GetComponent<Image>().fillAmount = 0.12f + ((health / maxHealth) * 0.26f);
        finishedLostCoroutine = true;

        InvokeRepeating("UpdateFPS", 0f, 1f);
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        equippedText.text = gunEquippedTexts[gunEquipped];

        playerHitTimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;

        //Cursor + Enemy Marker
        if (crosshairState == 3)
        {
            Cursor.SetCursor(crosshairImages[crosshairState], new Vector2(0, 0), CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(crosshairImages[crosshairState], new Vector2(crosshairImages[crosshairState].width / 2, crosshairImages[crosshairState].height / 2), CursorMode.ForceSoftware);
        }

        if (crosshairState != 0)
        {
            crosshairTimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;

            if (crosshairTimer >= crosshairResetTime)
            {
                crosshairTimer = 0;
                crosshairState = 0;
            }
        }

        foreach(Transform enemy in enemies.transform.GetChild(currentLevel - 1))
        {
            if (nearestEnemy == null)
            {
                nearestEnemy = enemy.gameObject;
            }
            else
            {
                if (enemy != nearestEnemy)
                {
                    float a = enemy.transform.position.x - player.transform.position.x;
                    float b = enemy.transform.position.y - player.transform.position.y;

                    float distance = Mathf.Sqrt((a * a) + (b * b));

                    //Current nearest enemy distance

                    float x = nearestEnemy.transform.position.x - player.transform.position.x;
                    float y = nearestEnemy.transform.position.y - player.transform.position.y;

                    float currentDistance = Mathf.Sqrt((x * x) + (y * y));


                    if (distance < currentDistance)
                    {
                        nearestEnemy = enemy.gameObject;
                    }
                }
            }
        }

        if (enemies.transform.GetChild(currentLevel - 1).childCount == 0)
        {
            nearestEnemy = null;
        }
        else
        {
            Vector3 new_pos = nearestEnemy.transform.position;

            Vector3 vec = new_pos - player.transform.position;
            float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;

            enemyMarker.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

            Vector2 screenPosition = Camera.main.WorldToScreenPoint(nearestEnemy.transform.position);

            if (screenPosition.x < -100 || screenPosition.x > (Screen.width + 100)
                || screenPosition.y < -100 || screenPosition.y > (Screen.height + 100))
            {
                enemyMarker.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                enemyMarker.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        if (playerHitTimer < 20)
        {
            Vignette vignette;
            postprocessing.GetComponent<UnityEngine.Rendering.Volume>().profile.TryGet(out vignette);
            vignette.color.value = new Color(1f, 0f, 0f);
        }
        else
        {
            Vignette vignette;
            postprocessing.GetComponent<UnityEngine.Rendering.Volume>().profile.TryGet(out vignette);
            vignette.color.value = new Color(0f, 0.5f, 1f);
        }

        if (musicTimer >= 0)
        {
            musicTimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;
        }
        if (gameTimer >= 0)
        {
            gameTimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;
        }

        if (gameTimer >= 260)
        {
            Music.PlaySound("game");
            gameTimer = -100f;
        }

        if (musicTimer >= 60)
        {
            Sound.PlaySound("respawn");
            musicTimer = -100f;
        }

        if (inShop)
        {
            crosshairState = 3;

            hairRGB = new Vector3(HRed.value, HGreen.value, HBlue.value);
            clothesRGB = new Vector3(CRed.value, CGreen.value, CBlue.value);

            shopHair.GetComponent<Image>().color = new Color(hairRGB.x, hairRGB.y, hairRGB.z);
            shopClothes.GetComponent<Image>().color = new Color(clothesRGB.x, clothesRGB.y, clothesRGB.z);

            healthLevelText.text = "Level " + healthLevel;

            if (healthLevel < 5)
            {
                healthBuyText.text = "Buy";
                healthPriceText.text = healthPrices[healthLevel] + " EC";

                if (money >= healthPrices[healthLevel])
                {
                    buyHealthButton.interactable = true;
                }
                else
                {
                    buyHealthButton.interactable = false;
                }
            }
            else
            {
                buyHealthButton.interactable = false;
                healthBuyText.text = "Max";
                healthPriceText.text = "Max";
            }

            if ((HRed.value != player.GetComponent<PlayerMove>().hair.GetComponent<SpriteRenderer>().color.r) ||
                (HGreen.value != player.GetComponent<PlayerMove>().hair.GetComponent<SpriteRenderer>().color.g) ||
                (HBlue.value != player.GetComponent<PlayerMove>().hair.GetComponent<SpriteRenderer>().color.b) ||
                (CRed.value != player.GetComponent<PlayerMove>().clothes.GetComponent<SpriteRenderer>().color.r) ||
                (CGreen.value != player.GetComponent<PlayerMove>().clothes.GetComponent<SpriteRenderer>().color.g) ||
                (CBlue.value != player.GetComponent<PlayerMove>().clothes.GetComponent<SpriteRenderer>().color.b))
            {
                if (money >= colourCost)
                {
                    applyColor.interactable = true;
                }
                else
                {
                    applyColor.interactable = false;
                }
            }
            else
            {
                applyColor.interactable = false;
            }

            for (int i = 0; i < 5; i++)
            {
                gunPriceTexts[i].text = gunPrices[i] + " EC";
                if (gunBought[i])
                {
                    if ((gunEquipped == i + 1) || (tempGun == i + 1))
                    {
                        //buyGuns[i].GetComponent<Image>().color = Color.white;
                        buyGuns[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Armed";
                        buyGuns[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 22;
                    }
                    else
                    {
                        //buyGuns[i].GetComponent<Image>().color = Color.white;
                        buyGuns[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip";
                        buyGuns[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 22;
                    }
                }
                else
                {
                    //buyGuns[i].GetComponent<Image>().color = Color.white;
                    buyGuns[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Buy";
                    buyGuns[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 36;

                    if (money >= gunPrices[i])
                    {
                        buyGuns[i].interactable = true;
                    }
                    else
                    {
                        buyGuns[i].interactable = false;
                    }
                }
            }
        }
        else
        {
            hairRGB = new Vector3(player.GetComponent<PlayerMove>().hair.GetComponent<SpriteRenderer>().color.r, player.GetComponent<PlayerMove>().hair.GetComponent<SpriteRenderer>().color.g, player.GetComponent<PlayerMove>().hair.GetComponent<SpriteRenderer>().color.b);
            clothesRGB = new Vector3(player.GetComponent<PlayerMove>().clothes.GetComponent<SpriteRenderer>().color.r, player.GetComponent<PlayerMove>().clothes.GetComponent<SpriteRenderer>().color.g, player.GetComponent<PlayerMove>().clothes.GetComponent<SpriteRenderer>().color.b);
            HRed.value = hairRGB.x;
            HGreen.value = hairRGB.y;
            HBlue.value = hairRGB.z;
            CRed.value = clothesRGB.x;
            CGreen.value = clothesRGB.y;
            CBlue.value = clothesRGB.z;
        }

        if (!isPaused)
        {
            if (!ultActive)
            {
                if (dashValue < 100)
                {
                    dashValue += 1f * 60f / (1f / Time.deltaTime); //Change first number for rate
                }
                else
                {
                    dashValue = 100;
                }
            }
            else
            {
                if (dashValue < 100)
                {
                    dashValue += 3.0f * 60f / (1f / Time.deltaTime); //Change first number for rate
                }
                else
                {
                    dashValue = 100;
                }
            }
            if (!ultActive)
            {
                if (ultValue >= 100)
                {
                    ultValue = 100;
                    ultReady = true;
                    ultParticles.SetActive(true);
                }
                else
                {
                    ultValue += 0.04f * 60f / (1f / Time.deltaTime); //Change first number for rate
                    ultReady = false;
                    ultParticles.SetActive(false);
                }
            }
            else
            {
                if (ultValue >= 100)
                {
                    ultValue = 100;
                }

                if (ultValue <= 0)
                {
                    ultValue = 0;
                    ultActive = false;
                    hidBrother = false;
                    player.GetComponent<PlayerMove>().brother.GetComponent<SoulFollow>().reachedPlayer = false;
                    //player.GetComponent<PlayerMove>().brother.GetComponent<SoulFollow>().agent.ResetPath();
                    Instantiate(ultPE, player.transform.position, Quaternion.identity);
                    BultPe.Stop();
                    UltimateUI.SetActive(false);
                    Sound.PlaySound("ult");

                    overlayUI.GetComponent<Overlay>().StopPulse();

                    Color x = player.GetComponent<PlayerMove>().brother.GetComponent<SpriteRenderer>().color;
                    x.a = 0.666f;
                    player.GetComponent<PlayerMove>().brother.GetComponent<SpriteRenderer>().color = x;

                    for (int i = 2; i < player.GetComponent<PlayerMove>().brother.transform.childCount; i++)
                    {
                        player.GetComponent<PlayerMove>().brother.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (!ultTransition)
                    {
                        if (!ultPaused)
                        {
                            ultValue -= 0.3f * 60f / (1f / Time.deltaTime); //Change first number for rate
                        }
                    }
                }
            }

            if (ohValue >= 100)
            {
                ohValue = 100f;
                overheated = true;
                ohParticles.SetActive(true);
                Sound.PlaySound("overheat");
            }

            if (ohValue > 0)
            {
                ohValue -= 1f * 60f / (1f / Time.deltaTime); //Change first number for rate
            }

            if (ohValue <= 0)
            {
                ohValue = 0;
                if (overheated)
                {
                    overheated = false;
                }
            }
        }
        else
        {
            crosshairState = 3;

            settings.GetComponent<Settings>().volume = volumeSlider.value;
            volumeText.text = (Convert.ToInt32(volumeSlider.value * 200)) + "%";

            AudioListener.volume = settings.GetComponent<Settings>().volume;
        }

        settings.GetComponent<Settings>().cells = money;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        currentLevel = settings.GetComponent<Settings>().currentLevel;

        if (health <= 0)
        {
            if (!deathPlayed)
            {
                deathPlayed = true;
                Sound.PlaySound("death");

                player.GetComponent<PlayerMove>().moveDisabled = true;
                player.GetComponent<PlayerMove>().movement = new Vector2(0f, 0f);

                Invoke("BeginFade", 0f);

                GameObject crack = Instantiate(screenCrack, new Vector3(0f, 0f), Quaternion.identity);
                crack.transform.SetParent(GameObject.Find("Canvas").transform);
                crack.transform.localPosition = Vector3.zero;
                crack.transform.localScale = Vector3.one;

                Invoke("ResetPlayer", 4f);
            }
        }

        int z = 10;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));

        if (!isPaused && !inShop)
        {
            cursor.transform.position = mousePosition;
        }

        ultBar.GetComponent<Image>().fillAmount = ultValue / 100;
        ohBar.GetComponent<Image>().fillAmount = ohValue / 100;
        dashBar.GetComponent<Image>().fillAmount = dashValue / 100;
        healthbar.GetComponent<Image>().fillAmount = 0.12f + ((health / maxHealth) * 0.26f);

        if (healthbar.GetComponent<Image>().fillAmount < losthealthbar.GetComponent<Image>().fillAmount)
        {
            if (lostHealthFillAmount != (losthealthbar.GetComponent<Image>().fillAmount - healthbar.GetComponent<Image>().fillAmount))
            {
                lostHealthFillAmount = losthealthbar.GetComponent<Image>().fillAmount - healthbar.GetComponent<Image>().fillAmount;

                if (finishedLostCoroutine)
                {
                    StartCoroutine(CatchUpHealth(lostHealthFillAmount, 1f));
                }
            }
        }
        else
        {
            losthealthbar.GetComponent<Image>().fillAmount = healthbar.GetComponent<Image>().fillAmount;
        }

        if (ultTransition)
        {
            if (player.GetComponent<PlayerMove>().brother.GetComponent<SoulFollow>().reachedPlayer && !hidBrother)
            {
                hidBrother = true;
                Instantiate(ultPE, player.transform.position, Quaternion.identity);
                player.GetComponent<PlayerMove>().brother.GetComponent<SoulFollow>().movement = new Vector2(0f, -1f);
                player.GetComponent<PlayerMove>().brother.GetComponent<SoulFollow>().movement = new Vector2(0f, 0f);
                Invoke("HideBrother", 0f);
                BultPe.Play();
                UltimateUI.SetActive(true);
            }
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (!isPaused && !player.GetComponent<PlayerMove>().isTeleporting && !player.GetComponent<PlayerMove>().moveDisabled)
            {
                if (ultReady)
                {
                    if (!ultActive)
                    {
                        ultReady = false;
                        ultActive = true;
                        overheated = false;
                        ohValue = 0f;

                        player.GetComponent<PlayerMove>().brother.GetComponent<SoulFollow>().ultDistance = player.GetComponent<PlayerMove>().brother.transform.position - player.transform.position;

                        player.GetComponent<PlayerMove>().isShooting = false;
                        player.GetComponent<PlayerMove>().moveDisabled = true;
                        player.GetComponent<PlayerMove>().movement = new Vector2(0f, -1f);
                        player.GetComponent<PlayerMove>().movement = new Vector2(0f, 0f);
                        ultTransition = true;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!inShop)
            {
                if (isPaused == false)
                {
                    Pause();
                    pausemenu.SetActive(true);
                    mainui.SetActive(false);
                }
                else
                {
                    Resume();
                    pausemenu.SetActive(false);
                    mainui.SetActive(true);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inShop)
            {
                ExitShop();
            }
            else if (shopInRange)
            {
                Sound.PlaySound("click");
                if (isPaused)
                {
                    pausemenu.SetActive(false);
                    mainui.SetActive(true);
                }
                Pause();
                shopmenu.SetActive(true);
                inShop = true;
                mainui.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) //REMOVE THIS
        {
            player.transform.position = settings.GetComponent<Settings>().levelPos[currentLevel - 1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) //REMOVE THIS
        {
            player.transform.position = new Vector2(-110f, -30f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) //REMOVE THIS
        {
            health = maxHealth;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) //REMOVE THIS
        {
            health = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) //REMOVE THIS
        {
            ultValue = 100f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) //REMOVE THIS
        {
            money += 10000;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)) //REMOVE THIS
        {
            money = 0;
        }

        /*
        if (Input.GetKeyDown(KeyCode.K)) //REMOVE THIS
        {
            if (Application.targetFrameRate == 60)
            {
                Application.targetFrameRate = 10;
            }
            else
            {
                Application.targetFrameRate = 60;
            }
        }
        if (Input.GetKeyDown(KeyCode.L)) //REMOVE THIS
        {
            ultValue = 100;
        }
        if (Input.GetKeyDown(KeyCode.H)) //REMOVE THIS
        {
            mainCamera.GetComponent<CameraShake>().Shake(0.05f, 2f);
        }
        if (Input.GetKeyDown(KeyCode.J)) //REMOVE THIS
        {
            player.transform.position = new Vector2(-110f, -30f);
        }
        if (Input.GetKeyDown(KeyCode.M)) //REMOVE THIS
        {
            player.transform.position = new Vector2(423f, 20f);
        }
        */

        if (shopInRange)
        {
            shopTexts.SetActive(true);
        }
        else
        {
            shopTexts.SetActive(false);
        }

        credCounter.text = money.ToString();
        shopcredCounter.text = "EC: " + money.ToString();

        if (currentLevel < 11)
        {
            killedCounter.text = killed + "/" + enemyNo[currentLevel - 1];
        }
    }

    void BeginFade()
    {
        faderobj.GetComponent<Fade>().Fader(2f, 0.5f);
    }
    void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        settings.GetComponent<Settings>().currentGun = tempGun;
        player.GetComponent<PlayerMove>().moveDisabled = true;
    }
    void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        player.GetComponent<PlayerMove>().moveDisabled = false;
    }
    public void PauseButton()
    {
        Sound.PlaySound("click");
        pausemenu.SetActive(true);
        mainui.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        player.GetComponent<PlayerMove>().moveDisabled = true;
    }
    public void UnpauseButton()
    {
        crosshairState = 0;

        Sound.PlaySound("click");
        pausemenu.SetActive(false);
        mainui.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        player.GetComponent<PlayerMove>().moveDisabled = false;
    }
    void ResetPlayer()
    {
        Sound.PlaySound("respawn");

        settings.GetComponent<Settings>().destination = settings.GetComponent<Settings>().levelPos[currentLevel - 1];
        settings.GetComponent<Settings>().currentGun = tempGun;
        settings.GetComponent<Settings>().hairRGB = hairRGB;
        settings.GetComponent<Settings>().clothesRGB = clothesRGB;
        settings.GetComponent<Settings>().gunBought = gunBought;
        settings.GetComponent<Settings>().cells = money;
        settings.GetComponent<Settings>().healthLevel = healthLevel;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        Sound.PlaySound("click");
        FadePauseMenu = true;
        Time.timeScale = 1f;
        faderobj.GetComponent<Fade>().Fader(2f, 0.5f);
        Invoke("Menu", 2f);

        settings.GetComponent<Settings>().currentGun = tempGun;
        settings.GetComponent<Settings>().hairRGB = hairRGB;
        settings.GetComponent<Settings>().clothesRGB = clothesRGB;
        settings.GetComponent<Settings>().gunBought = gunBought;
        settings.GetComponent<Settings>().cells = money;
        settings.GetComponent<Settings>().healthLevel = healthLevel;

        if (currentLevel > settings.GetComponent<Settings>().maxLevel)
        {
            settings.GetComponent<Settings>().maxLevel = currentLevel;
        }
    }
    public void SetPlayerColour()
    {
        if (money >= colourCost)
        {
            Sound.PlaySound("buy");
            money -= colourCost;
            player.GetComponent<PlayerMove>().hair.GetComponent<SpriteRenderer>().color = new Color(hairRGB.x, hairRGB.y, hairRGB.z);
            player.GetComponent<PlayerMove>().clothes.GetComponent<SpriteRenderer>().color = new Color(clothesRGB.x, clothesRGB.y, clothesRGB.z);
        }
    }
    public void BuyGun1()
    {
        if (!gunBought[0])
        {
            if (money >= gunPrices[0])
            {
                Sound.PlaySound("buy");
                money -= gunPrices[0];
                gunBought[0] = true;
                gunEquipped = 1;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
        else
        {
            if (gunEquipped != 1)
            {
                Sound.PlaySound("click");
                gunEquipped = 1;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
    }
    public void BuyGun2()
    {
        if (!gunBought[1])
        {
            if (money >= gunPrices[1])
            {
                Sound.PlaySound("buy");
                money -= gunPrices[1];
                gunBought[1] = true;
                gunEquipped = 2;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
        else
        {
            if (gunEquipped != 2)
            {
                Sound.PlaySound("click");
                gunEquipped = 2;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
    }
    public void BuyGun3()
    {
        if (!gunBought[2])
        {
            if (money >= gunPrices[2])
            {
                Sound.PlaySound("buy");
                money -= gunPrices[2];
                gunBought[2] = true;
                gunEquipped = 3;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
        else
        {
            if (gunEquipped != 3)
            {
                Sound.PlaySound("click");
                gunEquipped = 3;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
    }
    public void BuyGun4()
    {
        if (!gunBought[3])
        {
            if (money >= gunPrices[3])
            {
                Sound.PlaySound("buy");
                money -= gunPrices[3];
                gunBought[3] = true;
                gunEquipped = 4;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
        else
        {
            if (gunEquipped != 4)
            {
                Sound.PlaySound("click");
                gunEquipped = 4;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
    }
    public void BuyGun5()
    {
        if (!gunBought[4])
        {
            if (money >= gunPrices[4])
            {
                Sound.PlaySound("buy");
                money -= gunPrices[4];
                gunBought[4] = true;
                gunEquipped = 5;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
        else
        {
            if (gunEquipped != 5)
            {
                Sound.PlaySound("click");
                gunEquipped = 5;
                tempGun = gunEquipped;
                settings.GetComponent<Settings>().currentGun = tempGun;
            }
        }
    }
    public void BuyHealth()
    {
        if (healthLevel < 5)
        {
            if (money >= healthPrices[healthLevel])
            {
                Sound.PlaySound("buy");
                money -= healthPrices[healthLevel];
                maxHealth = maxHealth * 1.2f;
                health = maxHealth;
                healthLevel += 1;
            }
        }
    }
    void Menu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    void EndUltTransition()
    {
        ultTransition = false;
        player.GetComponent<PlayerMove>().moveDisabled = false;

        overlayUI.GetComponent<Overlay>().StartPulse();
    }
    void HideBrother()
    {
        Invoke("EndUltTransition", 0f);
        Color x = player.GetComponent<PlayerMove>().brother.GetComponent<SpriteRenderer>().color;
        x.a = 0f;
        player.GetComponent<PlayerMove>().brother.GetComponent<SpriteRenderer>().color = x;

        for (int i = 2; i < player.GetComponent<PlayerMove>().brother.transform.childCount; i++)
        {
            player.GetComponent<PlayerMove>().brother.transform.GetChild(i).gameObject.SetActive(false);
        }

        //player.GetComponent<PlayerMove>().brother.GetComponent<SoulFollow>().
    }

    IEnumerator CatchUpHealth(float fillAmount, float delay)
    {
        yield return new WaitForSeconds(delay);

        finishedLostCoroutine = false;

        if (fillAmount > 0f)
        {
            if (fillAmount >= 0.0005f)
            {
                losthealthbar.GetComponent<Image>().fillAmount -= 0.0005f * 60f / (1f / Time.deltaTime); //Change first number for rate
            }
            else
            {
                losthealthbar.GetComponent<Image>().fillAmount -= fillAmount;
            }

            float x = fillAmount - 0.0005f * 60f / (1f / Time.deltaTime); //Change first number for rate

            if (x > 0f)
            {
                StartCoroutine(CatchUpHealth(x, 0f));
            }
            else
            {
                finishedLostCoroutine = true;
            }
        }
    }

    public void DisableOHParticles()
    {
        ohParticles.SetActive(false);
    }

    void UpdateFPS()
    {
        fpsCounter.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }

    public void PlayClick()
    {
        Sound.PlaySound("click");
    }
    public void OpenLink(int LinkNumber)
    {
        if (LinkNumber == 1)
        {
            Application.OpenURL("https://www.instagram.com/troysart_/");
        }
        else if (LinkNumber == 2)
        {
            Application.OpenURL("https://www.instagram.com/enkrypt.ig/");
        }
        else if (LinkNumber == 3)
        {
            Application.OpenURL("https://www.instagram.com/heyvulp/");
        }
        else if (LinkNumber == 4)
        {
            Application.OpenURL("https://soundcloud.com/vulpmusic");
        }
        else if (LinkNumber == 5)
        {
            Application.OpenURL("https://enkrypt.itch.io/from-the-sky");
        }
        else if (LinkNumber == 6)
        {
            Application.OpenURL("https://www.youtube.com/channel/UCx66yD6ckAYRp6JHcm08-3w");
        }
    }

    public void ExitShop()
    {
        crosshairState = 0;

        Sound.PlaySound("click");
        Resume();
        shopmenu.SetActive(false);
        inShop = false;
        mainui.SetActive(true);
    }

    public void PauseUltimate()
    {
        ultPaused = true;
    }

    public void UnpauseUltimate()
    {
        ultPaused = false;
    }

    public void InitiateEEPopUp(int number)
    {
        EEHandler.GetComponent<EEPopUp>().BeginPopUp(number);
    }
}
