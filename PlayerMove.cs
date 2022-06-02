using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public GameHandler handler;

    public float speed = 0.0f;
    public float dashspeed;
    public Vector2 movement;
    public Animator animator;
    public Animator clothesAnimator;

    public float dashLength;
    public bool dashFadingOut;
    private float dashAlpha;
    private int enemyCols;

    private Rigidbody2D rb;
    private GameObject hand;
    private GameObject gun;
    public GameObject hair;
    private GameObject chest;
    private GameObject shoulder;
    private GameObject visor;
    public GameObject clothes;

    public GameObject brother;

    public Sprite[] allGuns;
    public Sprite[] allHair;
    public Sprite[] allChest;
    public Sprite[] allSho;
    public Sprite[] allVisor;

    public GameObject bullet;
    private Vector2 tempcursorpos;
    public float shootTime;
    private float shootTimer;

    public GameObject followcam;

    public bool moveDisabled;
    public bool isShooting;

    public bool isTeleporting;
    public Vector3 tpPos;
    public float tpAlpha;

    public bool TwoKeys;
    public bool animSwitched;
    public int DiagSwapTime;
    private float DiagSwapTimer;

    public bool isDashing;
    private bool isDashTransitioning;
    private float dashTimer;
    public bool dashBoost;

    public bool touchingDashWall; //Is the player touching a dashable wall OR crate
    public bool cancelDash;

    public float HframeX;
    public float HframeY;

    public float ohInc;

    private int slashNumber;
    private float inactivity;

    private int nextTpLevel; //Variable storing the level variable of the last teleporter the player collided with

    public GameObject currentNode;

    private GameObject currentDashObj;

    // Start is called before the first frame update
    void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        rb = GetComponent<Rigidbody2D>();
        hand = transform.GetChild(0).gameObject;
        gun = transform.GetChild(1).gameObject;
        hair = transform.GetChild(2).gameObject;
        visor = hair.transform.GetChild(0).gameObject;
        chest = transform.GetChild(3).gameObject;
        shoulder = transform.GetChild(4).gameObject;
        clothes = transform.GetChild(5).gameObject;

        moveDisabled = false;
        isDashing = false;
        dashTimer = 0;

        shootTimer = shootTime;
        DiagSwapTimer = 0;

        HframeX = 0;
        HframeY = 0;

        tpAlpha = 1f;

        transform.position = new Vector3(handler.settings.GetComponent<Settings>().destination.x, handler.settings.GetComponent<Settings>().destination.y);

        hair.GetComponent<SpriteRenderer>().color = new Color(handler.settings.GetComponent<Settings>().hairRGB.x, handler.settings.GetComponent<Settings>().hairRGB.y, handler.settings.GetComponent<Settings>().hairRGB.z);
        clothes.GetComponent<SpriteRenderer>().color = new Color(handler.settings.GetComponent<Settings>().clothesRGB.x, handler.settings.GetComponent<Settings>().clothesRGB.y, handler.settings.GetComponent<Settings>().clothesRGB.z);
    }

    // Update is called once per frame
    void Update()
    {
        allGuns = handler.allGuns[handler.gunEquipped];

        shootTime = handler.gunFirerates[handler.gunEquipped];
        ohInc = handler.ohIncrements[handler.gunEquipped];

        if (handler.ultActive)
        {
            shootTime = handler.gunFirerates[handler.gunEquipped] * 0.6f;
            ohInc = 0f;
        }

        clothesAnimator.SetInteger("AnimationNo", animator.GetInteger("AnimationNo"));
        
        if (animator.GetInteger("AnimationNo") == 1 || animator.GetInteger("AnimationNo") == 5)
        {
            clothes.transform.localPosition = new Vector3(0f, -0.374f);

        }
        else if (animator.GetInteger("AnimationNo") == 2 || animator.GetInteger("AnimationNo") == 6)
        {
            clothes.transform.localPosition = new Vector3(-0.0318f, -0.375f);
        }
        else if (animator.GetInteger("AnimationNo") == 3 || animator.GetInteger("AnimationNo") == 7)
        {
            clothes.transform.localPosition = new Vector3(0f, -0.34f);
        }
        else if (animator.GetInteger("AnimationNo") == 4 || animator.GetInteger("AnimationNo") == 8)
        {
            clothes.transform.localPosition = new Vector3(0.0313f, -0.374f);
        }
        else
        {
            clothes.transform.localPosition = new Vector3(0f, -0.34f);
        }
        

        if (isTeleporting)
        {
            if (tpAlpha >= 0.005f)
            {
                tpAlpha -= 0.005f * 60f / (1f / Time.deltaTime); //Change first number for rate;
            }

            Color x = GetComponent<SpriteRenderer>().color;
            x.a = tpAlpha;
            GetComponent<SpriteRenderer>().color = x;

            foreach (Transform child in transform)
            {
                if (child.gameObject.name != "Shadow")
                {
                    Color y = child.GetComponent<SpriteRenderer>().color;
                    y.a = tpAlpha;
                    child.GetComponent<SpriteRenderer>().color = y;

                    if (child.gameObject.name == "Hair")
                    {
                        Color z = child.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                        z.a = tpAlpha;
                        child.transform.GetChild(0).GetComponent<SpriteRenderer>().color = z;
                    }
                }
            }
        }
        if (!moveDisabled && !handler.isPaused && !handler.inShop)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space) && handler.dashValue >= 100)
            {
                handler.dashValue = 0f;
                isDashTransitioning = true;
                isDashing = true;
                dashFadingOut = true;
                dashAlpha = 1f;
                dashBoost = true;

                /*
                int dashParticleType = Random.Range(1, 4);

                GameObject particle = Instantiate(handler.dashParticles, transform.position, Quaternion.identity);
                particle.GetComponent<Animator>().SetInteger("Number", dashParticleType);
                particle.transform.SetParent(GameObject.Find("Camera").transform);
                particle.transform.localPosition = new Vector3(0f, 0f, 10f);

                GameObject glow = Instantiate(handler.dashGlow, transform.position, Quaternion.identity);
                glow.transform.SetParent(GameObject.Find("Camera").transform);
                glow.transform.localPosition = new Vector3(0f, 0f, 10f);
                */

                Instantiate(handler.warpObj, transform.position, Quaternion.identity);

                /*GameObject dash = Instantiate(handler.dashObj, transform.position, Quaternion.identity);

                currentDashObj = dash;

                Vector3 new_pos = handler.cursor.transform.position;

                Vector3 vec = new_pos - dash.transform.position;
                float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
                dash.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                */

                Invoke("TriggerDashFadeIn", dashLength - 0.01f);
                Invoke("StopDash", dashLength);
            }
        }
        else
        {
            //Move is disabled.
        }

        /*
        if (frame == 0)
        {
            hair.transform.localPosition = new Vector3(hair.transform.localPosition.x, 0.374f);
        }
        else if (frame == 1)
        {
            hair.transform.localPosition = new Vector3(hair.transform.localPosition.x, 0.436f);
        }
        else if (frame == -1)
        {
            hair.transform.localPosition = new Vector3(hair.transform.localPosition.x, 0.311f);
        }
        */
        if (visor.GetComponent<SpriteRenderer>().sprite == allVisor[1])
        {
            visor.transform.localPosition = new Vector3(-0.218f, -0.281f);
        }
        else
        {
            visor.transform.localPosition = new Vector3(0.188f, -0.218f);
        }

        {

        }

        if (TwoKeys)
        {
            DiagSwapTimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;
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
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[0];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[0];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[0];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[0];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
                    }
                    else
                    {
                        animator.SetInteger("AnimationNo", 2);
                        animSwitched = true;
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[1];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[1];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[1];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[1];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];

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
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[1];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[1];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[1];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[1];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
                    }
                    else
                    {
                        animator.SetInteger("AnimationNo", 3);
                        animSwitched = true;
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[2];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[2];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[2];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[2];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 4;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[0];
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
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[2];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[2];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[2];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[2];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 4;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[0];
                    }
                    else
                    {
                        animator.SetInteger("AnimationNo", 4);
                        animSwitched = true;
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[3];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[3];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[3];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[3];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[1];
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
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[3];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[3];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[3];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[3];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[1];
                    }
                    else
                    {
                        animator.SetInteger("AnimationNo", 1);
                        animSwitched = true;
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[0];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[0];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[0];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[0];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
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
            Color tmpcolor = gun.GetComponent<SpriteRenderer>().color;
            tmpcolor.a = 0f;
            gun.GetComponent<SpriteRenderer>().color = tmpcolor;

            if (!(movement.x == 0f && movement.y == 0f))
            {
                //hand.transform.localPosition = new Vector3(movement.x * 0.6f, movement.y * 0.6f);
            }
            if (!TwoKeys)
            {
                if (movement.x == 0 && movement.y == 0)
                {
                    animator.SetInteger("AnimationNo", 0);
                    gun.GetComponent<SpriteRenderer>().sprite = allGuns[2];
                    hair.GetComponent<SpriteRenderer>().sprite = allHair[2];
                    chest.GetComponent<SpriteRenderer>().sprite = allChest[2];
                    shoulder.GetComponent<SpriteRenderer>().sprite = allSho[2];

                    gun.GetComponent<SpriteRenderer>().sortingOrder = 4;
                    visor.GetComponent<SpriteRenderer>().sprite = allVisor[0];
                }
                else
                {
                    if (movement.x == 1)
                    {
                        animator.SetInteger("AnimationNo", 2);
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[1];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[1];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[1];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[1];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
                    }
                    else if (movement.x == -1)
                    {
                        animator.SetInteger("AnimationNo", 4);
                        gun.GetComponent<SpriteRenderer>().sprite = allGuns[3];
                        hair.GetComponent<SpriteRenderer>().sprite = allHair[3];
                        chest.GetComponent<SpriteRenderer>().sprite = allChest[3];
                        shoulder.GetComponent<SpriteRenderer>().sprite = allSho[3];

                        gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        visor.GetComponent<SpriteRenderer>().sprite = allVisor[1];
                    }
                    else
                    {
                        if (movement.y == 1)
                        {
                            animator.SetInteger("AnimationNo", 1);
                            gun.GetComponent<SpriteRenderer>().sprite = allGuns[0];
                            hair.GetComponent<SpriteRenderer>().sprite = allHair[0];
                            chest.GetComponent<SpriteRenderer>().sprite = allChest[0];
                            shoulder.GetComponent<SpriteRenderer>().sprite = allSho[0];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                            visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
                        }
                        else if (movement.y == -1)
                        {
                            animator.SetInteger("AnimationNo", 3);
                            gun.GetComponent<SpriteRenderer>().sprite = allGuns[2];
                            hair.GetComponent<SpriteRenderer>().sprite = allHair[2];
                            chest.GetComponent<SpriteRenderer>().sprite = allChest[2];
                            shoulder.GetComponent<SpriteRenderer>().sprite = allSho[2];

                            gun.GetComponent<SpriteRenderer>().sortingOrder = 4;
                            visor.GetComponent<SpriteRenderer>().sprite = allVisor[0];
                        }
                    }
                }
            }
        }
        else //If you are shooting
        {
            if (!moveDisabled)
            {
                if (!isDashing)
                {
                    Color tmpcolor = gun.GetComponent<SpriteRenderer>().color;
                    tmpcolor.a = 1f;
                    gun.GetComponent<SpriteRenderer>().color = tmpcolor;
                }

                Vector3 pos = handler.cursor.transform.position - transform.position;
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
                                gun.GetComponent<SpriteRenderer>().sprite = allGuns[1];
                                hair.GetComponent<SpriteRenderer>().sprite = allHair[1];
                                chest.GetComponent<SpriteRenderer>().sprite = allChest[5];
                                shoulder.GetComponent<SpriteRenderer>().sprite = allSho[5];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
                            }
                        }
                        else //Top Quadrant
                        {
                            hand.transform.localPosition = new Vector3(0f, 0.6f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 5);
                                gun.GetComponent<SpriteRenderer>().sprite = allGuns[0];
                                hair.GetComponent<SpriteRenderer>().sprite = allHair[0];
                                chest.GetComponent<SpriteRenderer>().sprite = allChest[4];
                                shoulder.GetComponent<SpriteRenderer>().sprite = allSho[4];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
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
                                    gun.GetComponent<SpriteRenderer>().sprite = allGuns[1];
                                    hair.GetComponent<SpriteRenderer>().sprite = allHair[1];
                                    chest.GetComponent<SpriteRenderer>().sprite = allChest[5];
                                    shoulder.GetComponent<SpriteRenderer>().sprite = allSho[5];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                    visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
                                }
                                else
                                {
                                    animator.SetInteger("AnimationNo", 5);
                                    animSwitched = true;
                                    gun.GetComponent<SpriteRenderer>().sprite = allGuns[0];
                                    hair.GetComponent<SpriteRenderer>().sprite = allHair[0];
                                    chest.GetComponent<SpriteRenderer>().sprite = allChest[4];
                                    shoulder.GetComponent<SpriteRenderer>().sprite = allSho[4];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                    visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
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
                                gun.GetComponent<SpriteRenderer>().sprite = allGuns[1];
                                hair.GetComponent<SpriteRenderer>().sprite = allHair[1];
                                chest.GetComponent<SpriteRenderer>().sprite = allChest[5];
                                shoulder.GetComponent<SpriteRenderer>().sprite = allSho[5];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
                            }
                        }
                        else //Bottom Quadrant
                        {
                            hand.transform.localPosition = new Vector3(0f, -0.6f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 7);
                                gun.GetComponent<SpriteRenderer>().sprite = allGuns[2];
                                hair.GetComponent<SpriteRenderer>().sprite = allHair[2];
                                chest.GetComponent<SpriteRenderer>().sprite = allChest[6];
                                shoulder.GetComponent<SpriteRenderer>().sprite = allSho[6];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 4;
                                visor.GetComponent<SpriteRenderer>().sprite = allVisor[0];
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
                                    gun.GetComponent<SpriteRenderer>().sprite = allGuns[1];
                                    hair.GetComponent<SpriteRenderer>().sprite = allHair[1];
                                    chest.GetComponent<SpriteRenderer>().sprite = allChest[5];
                                    shoulder.GetComponent<SpriteRenderer>().sprite = allSho[5];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                    visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
                                }
                                else
                                {
                                    animator.SetInteger("AnimationNo", 7);
                                    animSwitched = true;
                                    gun.GetComponent<SpriteRenderer>().sprite = allGuns[2];
                                    hair.GetComponent<SpriteRenderer>().sprite = allHair[2];
                                    chest.GetComponent<SpriteRenderer>().sprite = allChest[6];
                                    shoulder.GetComponent<SpriteRenderer>().sprite = allSho[6];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 4;
                                    visor.GetComponent<SpriteRenderer>().sprite = allVisor[0];
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
                                gun.GetComponent<SpriteRenderer>().sprite = allGuns[3];
                                hair.GetComponent<SpriteRenderer>().sprite = allHair[3];
                                chest.GetComponent<SpriteRenderer>().sprite = allChest[7];
                                shoulder.GetComponent<SpriteRenderer>().sprite = allSho[7];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                visor.GetComponent<SpriteRenderer>().sprite = allVisor[1];
                            }
                        }
                        else //Top quadrant
                        {
                            hand.transform.localPosition = new Vector3(0f, 0.6f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 5);
                                gun.GetComponent<SpriteRenderer>().sprite = allGuns[0];
                                hair.GetComponent<SpriteRenderer>().sprite = allHair[0];
                                chest.GetComponent<SpriteRenderer>().sprite = allChest[4];
                                shoulder.GetComponent<SpriteRenderer>().sprite = allSho[4];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
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
                                    gun.GetComponent<SpriteRenderer>().sprite = allGuns[3];
                                    hair.GetComponent<SpriteRenderer>().sprite = allHair[3];
                                    chest.GetComponent<SpriteRenderer>().sprite = allChest[7];
                                    shoulder.GetComponent<SpriteRenderer>().sprite = allSho[7];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                    visor.GetComponent<SpriteRenderer>().sprite = allVisor[1];
                                }
                                else
                                {
                                    animator.SetInteger("AnimationNo", 5);
                                    animSwitched = true;
                                    gun.GetComponent<SpriteRenderer>().sprite = allGuns[0];
                                    hair.GetComponent<SpriteRenderer>().sprite = allHair[0];
                                    chest.GetComponent<SpriteRenderer>().sprite = allChest[4];
                                    shoulder.GetComponent<SpriteRenderer>().sprite = allSho[4];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                    visor.GetComponent<SpriteRenderer>().sprite = allVisor[2];
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
                                gun.GetComponent<SpriteRenderer>().sprite = allGuns[3];
                                hair.GetComponent<SpriteRenderer>().sprite = allHair[3];
                                chest.GetComponent<SpriteRenderer>().sprite = allChest[7];
                                shoulder.GetComponent<SpriteRenderer>().sprite = allSho[7];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                visor.GetComponent<SpriteRenderer>().sprite = allVisor[1];
                            }
                        }
                        else //Bottom quadrant
                        {
                            hand.transform.localPosition = new Vector3(0f, -0.6f);
                            if (!TwoKeys)
                            {
                                animator.SetInteger("AnimationNo", 7);
                                gun.GetComponent<SpriteRenderer>().sprite = allGuns[2];
                                hair.GetComponent<SpriteRenderer>().sprite = allHair[2];
                                chest.GetComponent<SpriteRenderer>().sprite = allChest[6];
                                shoulder.GetComponent<SpriteRenderer>().sprite = allSho[6];

                                gun.GetComponent<SpriteRenderer>().sortingOrder = 4;
                                visor.GetComponent<SpriteRenderer>().sprite = allVisor[0];
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
                                    gun.GetComponent<SpriteRenderer>().sprite = allGuns[3];
                                    hair.GetComponent<SpriteRenderer>().sprite = allHair[3];
                                    chest.GetComponent<SpriteRenderer>().sprite = allChest[7];
                                    shoulder.GetComponent<SpriteRenderer>().sprite = allSho[7];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 0;
                                    visor.GetComponent<SpriteRenderer>().sprite = allVisor[1];
                                }
                                else
                                {
                                    animator.SetInteger("AnimationNo", 7);
                                    animSwitched = true;
                                    gun.GetComponent<SpriteRenderer>().sprite = allGuns[2];
                                    hair.GetComponent<SpriteRenderer>().sprite = allHair[2];
                                    chest.GetComponent<SpriteRenderer>().sprite = allChest[6];
                                    shoulder.GetComponent<SpriteRenderer>().sprite = allSho[6];

                                    gun.GetComponent<SpriteRenderer>().sortingOrder = 4;
                                    visor.GetComponent<SpriteRenderer>().sprite = allVisor[0];
                                }
                            }
                        }
                    }
                }
            }
            if (hand.transform.localPosition.x > -10 && hand.transform.localPosition.x < 10 && hand.transform.localPosition.y > -10 && hand.transform.localPosition.y < 10)
            {
                hand.transform.localPosition = new Vector3(hand.transform.localPosition.x * handler.barrelPosX[handler.gunEquipped], hand.transform.localPosition.y * handler.barrelPosY[handler.gunEquipped]);
            }
            /*
            if (movement.x == 0 && movement.y == 0)
            {
                //animator.SetInteger("AnimationNo", 0);
            }
            else
            {
                if (movement.x == 1)
                {
                    animator.SetInteger("AnimationNo", 6);
                }
                else if (movement.x == -1)
                {
                    animator.SetInteger("AnimationNo", 8);
                }
                else
                {
                    if (movement.y == 1)
                    {
                        animator.SetInteger("AnimationNo", 5);
                    }
                    else if (movement.y == -1)
                    {
                        animator.SetInteger("AnimationNo", 7);
                    }
                }
            }
            */
        }

        //followcam.transform.position = new Vector3(transform.position.x, transform.position.y, followcam.transform.position.z);

        shootTimer -= Time.deltaTime;
        if (shootTimer < 0)
        {
            shootTimer = 0;
        }

        if (moveDisabled == false)
        {
            if ((!handler.overheated || handler.gunEquipped == 6) && !handler.isPaused && !handler.inShop)
            {
                if (Input.GetMouseButtonDown(0)) //If clicked but not holding down
                {
                    inactivity = 0;
                    isShooting = true;

                    if (shootTimer <= 0)
                    {
                        if (handler.gunEquipped == 2) //Shotgun
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                tempcursorpos = new Vector2(handler.cursor.transform.position.x, handler.cursor.transform.position.y);

                                //Instantiate(handler.flashPE, hand.transform.position, Quaternion.identity);
                                GameObject shottyproj = Instantiate(handler.bullets[handler.gunEquipped], hand.transform.position, Quaternion.identity);
                                tempcursorpos = tempcursorpos - new Vector2(shottyproj.transform.position.x, shottyproj.transform.position.y);
                                shottyproj.GetComponent<Projectile>().destination = tempcursorpos;
                                shottyproj.GetComponent<Projectile>().ignore = gameObject;
                            }
                        }
                        else
                        {
                            if (handler.gunEquipped == 6) //Katana
                            {
                                gun.GetComponent<SpriteRenderer>().enabled = false;
                                Invoke("EnableGun", 0.15f);
                            }
                            tempcursorpos = new Vector2(handler.cursor.transform.position.x, handler.cursor.transform.position.y);

                            //Instantiate(handler.flashPE, hand.transform.position, Quaternion.identity);
                            GameObject proj = Instantiate(handler.bullets[handler.gunEquipped], hand.transform.position, Quaternion.identity);
                            tempcursorpos = tempcursorpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                            proj.GetComponent<Projectile>().destination = tempcursorpos;
                            proj.GetComponent<Projectile>().ignore = gameObject;

                            if (proj.name == "WaveBullet(Clone)")
                            {
                                proj.GetComponent<Projectile>().InvokeSpawnSiblings();
                            }
                        }

                        shootTimer = shootTime;

                        if (handler.ohValue <= (100f - ohInc))
                        {
                            handler.ohValue += ohInc;
                        }
                        else if (handler.ohValue < 100f)
                        {
                            handler.ohValue = 100f;
                        }

                        if (handler.gunEquipped == 0) //All guns (automatic + manual)
                        {
                            int sound = Random.Range(0, 5);

                            if (sound == 0)
                            {
                                Sound.PlaySound("pistol1");
                            }
                            else if (sound == 1)
                            {
                                Sound.PlaySound("pistol2");
                            }
                            else if (sound == 2)
                            {
                                Sound.PlaySound("pistol3");
                            }
                            else if (sound == 3)
                            {
                                Sound.PlaySound("pistol4");
                            }
                            else if (sound == 4)
                            {
                                Sound.PlaySound("pistol5");
                            }
                        }
                        else if (handler.gunEquipped == 1)
                        {
                            Sound.PlaySound("machinegun");
                        }
                        else if (handler.gunEquipped == 2)
                        {
                            int sound = Random.Range(0, 5);

                            if (sound == 0)
                            {
                                Sound.PlaySound("shotty1");
                            }
                            else if (sound == 1)
                            {
                                Sound.PlaySound("shotty2");
                            }
                            else if (sound == 2)
                            {
                                Sound.PlaySound("shotty3");
                            }
                            else if (sound == 3)
                            {
                                Sound.PlaySound("shotty4");
                            }
                            else if (sound == 4)
                            {
                                Sound.PlaySound("shotty5");
                            }
                        }
                        else if (handler.gunEquipped == 3)
                        {
                            Sound.PlaySound("wavegun");
                        }
                        else if (handler.gunEquipped == 4)
                        {
                            Sound.PlaySound("handcannon");
                        }
                        else if (handler.gunEquipped == 5)
                        {
                            Sound.PlaySound("launcher");
                        }
                        else if (handler.gunEquipped == 6)
                        {
                            int sound = Random.Range(0, 4);

                            if (sound == 0)
                            {
                                Sound.PlaySound("slash1");
                            }
                            else if (sound == 1)
                            {
                                Sound.PlaySound("slash2");
                            }
                            else if (sound == 2)
                            {
                                Sound.PlaySound("slash3");
                            }
                            else if (sound == 3)
                            {
                                Sound.PlaySound("slash4");
                            }
                        }
                    }
                }
                else if (Input.GetMouseButton(0)) //If holding down
                {
                    inactivity = 0;
                    isShooting = true;

                    if (shootTimer <= 0)
                    {
                        if (handler.gunEquipped != 4 && handler.gunEquipped != 5 && handler.gunEquipped != 6) //Only automatic guns
                        {
                            if (handler.gunEquipped == 2) //Shotgun
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    tempcursorpos = new Vector2(handler.cursor.transform.position.x, handler.cursor.transform.position.y);

                                    //Instantiate(handler.flashPE, hand.transform.position, Quaternion.identity);
                                    GameObject shottyproj = Instantiate(handler.bullets[handler.gunEquipped], hand.transform.position, Quaternion.identity);
                                    tempcursorpos = tempcursorpos - new Vector2(shottyproj.transform.position.x, shottyproj.transform.position.y);
                                    shottyproj.GetComponent<Projectile>().destination = tempcursorpos;
                                    shottyproj.GetComponent<Projectile>().ignore = gameObject;
                                }
                            }
                            else
                            {
                                tempcursorpos = new Vector2(handler.cursor.transform.position.x, handler.cursor.transform.position.y);

                                //Instantiate(handler.flashPE, hand.transform.position, Quaternion.identity);
                                GameObject proj = Instantiate(handler.bullets[handler.gunEquipped], hand.transform.position, Quaternion.identity);
                                tempcursorpos = tempcursorpos - new Vector2(proj.transform.position.x, proj.transform.position.y);
                                proj.GetComponent<Projectile>().destination = tempcursorpos;
                                proj.GetComponent<Projectile>().ignore = gameObject;

                                if (proj.name == "WaveBullet(Clone)")
                                {
                                    proj.GetComponent<Projectile>().InvokeSpawnSiblings();
                                }
                            }

                            shootTimer = shootTime;

                            if (handler.ohValue <= (100f - ohInc))
                            {
                                handler.ohValue += ohInc;
                            }
                            else if (handler.ohValue < 100f)
                            {
                                handler.ohValue = 100f;
                            }

                            if (handler.gunEquipped == 0) //All guns (automatic + manual)
                            {
                                int sound = Random.Range(0, 5);

                                if (sound == 0)
                                {
                                    Sound.PlaySound("pistol1");
                                }
                                else if (sound == 1)
                                {
                                    Sound.PlaySound("pistol2");
                                }
                                else if (sound == 2)
                                {
                                    Sound.PlaySound("pistol3");
                                }
                                else if (sound == 3)
                                {
                                    Sound.PlaySound("pistol4");
                                }
                                else if (sound == 4)
                                {
                                    Sound.PlaySound("pistol5");
                                }
                            }
                            else if (handler.gunEquipped == 1)
                            {
                                Sound.PlaySound("machinegun");
                            }
                            else if (handler.gunEquipped == 2)
                            {
                                int sound = Random.Range(0, 5);

                                if (sound == 0)
                                {
                                    Sound.PlaySound("shotty1");
                                }
                                else if (sound == 1)
                                {
                                    Sound.PlaySound("shotty2");
                                }
                                else if (sound == 2)
                                {
                                    Sound.PlaySound("shotty3");
                                }
                                else if (sound == 3)
                                {
                                    Sound.PlaySound("shotty4");
                                }
                                else if (sound == 4)
                                {
                                    Sound.PlaySound("shotty5");
                                }
                            }
                            else if (handler.gunEquipped == 3)
                            {
                                Sound.PlaySound("wavegun");
                            }
                            else if (handler.gunEquipped == 4)
                            {
                                Sound.PlaySound("handcannon");
                            }
                            else if (handler.gunEquipped == 5)
                            {
                                Sound.PlaySound("launcher");
                            }
                            else if (handler.gunEquipped == 6)
                            {
                                int sound = Random.Range(0, 4);

                                if (sound == 0)
                                {
                                    Sound.PlaySound("slash1");
                                }
                                else if (sound == 1)
                                {
                                    Sound.PlaySound("slash2");
                                }
                                else if (sound == 2)
                                {
                                    Sound.PlaySound("slash3");
                                }
                                else if (sound == 3)
                                {
                                    Sound.PlaySound("slash4");
                                }
                            }
                        }
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    /*
                    //Debug.Log("CLICKED");
                    GameObject emp = Instantiate(handler.emp, transform.position, Quaternion.identity);

                    tempcursorpos = new Vector2(handler.cursor.transform.position.x, handler.cursor.transform.position.y);
                    tempcursorpos = tempcursorpos - new Vector2(emp.transform.position.x, emp.transform.position.y);
                    emp.GetComponent<Projectile>().destination = tempcursorpos;
                    //emp.GetComponent<Projectile>().ignore = gameObject;
                    */
                }
                else
                {
                    inactivity += 1f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                    if (inactivity > 60)
                    {
                        isShooting = false;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                inactivity = 0;
                if (handler.gunEquipped != 6)
                {
                    isShooting = true;
                    handler.tempGun = handler.gunEquipped;
                    handler.gunEquipped = 6;
                }
                else
                {
                    handler.gunEquipped = handler.tempGun;
                }
            }

            /*
            if (isDashing)
            {
                dashTimer += 1;

                if (dashTimer >= 5)
                {
                    isDashing = false;
                    dashTimer = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (movement.x != 0)
                {
                    Instantiate(handler.dashPE, transform.position, Quaternion.identity);
                    rb.MovePosition(rb.position + new Vector2(movement.x, 0) * speed * 10 * Time.fixedDeltaTime);
                    isDashing = true;
                }
                if (movement.y != 0)
                {
                    Instantiate(handler.dashPE, transform.position, Quaternion.identity);
                    rb.MovePosition(rb.position + new Vector2(0, movement.y) * speed * 10 * Time.fixedDeltaTime);
                    isDashing = true;
                }
            }
            */
        }
    }
    void FixedUpdate()
    {
        //Debug.Log(cancelDash);
        if (!isDashing)
        {
            Physics2D.IgnoreLayerCollision(8, 11, false); //Not efficient to do every frame

            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
            rb.isKinematic = false;
            GetComponent<Collider2D>().isTrigger = false;

            cancelDash = false;

            if (isDashTransitioning)
            {
                if (dashAlpha < 1f)
                {
                    dashAlpha += 0.05f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }
                else
                {
                    isDashTransitioning = false;
                }

                Color x = GetComponent<SpriteRenderer>().color;
                x.a = dashAlpha;
                GetComponent<SpriteRenderer>().color = x;

                foreach (Transform child in transform)
                {
                    if (child.gameObject.name != "Shadow")
                    {
                        Color y = child.GetComponent<SpriteRenderer>().color;
                        y.a = dashAlpha;
                        child.GetComponent<SpriteRenderer>().color = y;

                        if (child.gameObject.name == "Hair")
                        {
                            Color z = child.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                            z.a = dashAlpha;
                            child.transform.GetChild(0).GetComponent<SpriteRenderer>().color = z;
                        }
                    }
                }
            }
        }
        else
        {
            Dash();
        }
    }

    void Dash()
    {
        //Physics2D.IgnoreLayerCollision(8, 11, true);

        if ((enemyCols > 0 || touchingDashWall) && !cancelDash)
        {
            rb.isKinematic = true;
            GetComponent<Collider2D>().isTrigger = true;
        }
        else
        {
            rb.isKinematic = false;
            GetComponent<Collider2D>().isTrigger = false;
        }

        if (dashFadingOut)
        {
            dashAlpha = 0f;
            /*
            if (dashAlpha > 0.25f)
            {
                dashAlpha -= 0.25f;
            }
            */
        }

        Color x = GetComponent<SpriteRenderer>().color;
        x.a = dashAlpha;
        GetComponent<SpriteRenderer>().color = x;

        foreach (Transform child in transform)
        {
            if (child.gameObject.name != "Shadow")
            {
                Color y = child.GetComponent<SpriteRenderer>().color;
                y.a = dashAlpha;
                child.GetComponent<SpriteRenderer>().color = y;

                if (child.gameObject.name == "Hair")
                {
                    Color z = child.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                    z.a = dashAlpha;
                    child.transform.GetChild(0).GetComponent<SpriteRenderer>().color = z;
                }
            }
        }

        Vector2 dashmovement;

        dashmovement.x = ((int)handler.cursor.transform.position.x - transform.position.x);
        dashmovement.y = ((int)handler.cursor.transform.position.y - transform.position.y);

        if (dashmovement.x > 0f) //Right Side
        {
            if (dashmovement.y > 0f) //Top Right
            {
                if (dashmovement.x > dashmovement.y)
                {
                    dashmovement = new Vector2(1.0f, dashmovement.y / dashmovement.x);
                }
                else
                {
                    dashmovement = new Vector2(dashmovement.x / dashmovement.y, 1.0f);
                }
            }
            else //Bottom Right
            {
                if (dashmovement.x > -dashmovement.y)
                {
                    dashmovement = new Vector2(1.0f, dashmovement.y / dashmovement.x);
                }
                else
                {
                    dashmovement = new Vector2(dashmovement.x / -dashmovement.y, -1.0f);
                }
            }
        }
        else //Left Side
        {
            if (dashmovement.y > 0f) //Top Left
            {
                if (-dashmovement.x > dashmovement.y)
                {
                    dashmovement = new Vector2(-1.0f, dashmovement.y / -dashmovement.x);
                }
                else
                {
                    dashmovement = new Vector2(dashmovement.x / dashmovement.y, 1.0f);
                }
            }
            else //Bottom Left
            {
                if (-dashmovement.x > -dashmovement.y)
                {
                    dashmovement = new Vector2(-1.0f, -dashmovement.y / dashmovement.x);
                }
                else
                {
                    dashmovement = new Vector2(-dashmovement.x / dashmovement.y, -1.0f);
                }
            }
        }
        /*
        if (dashmovement.x > dashmovement.y)
        {
            if (dashmovement.x > 0f)
            {
                dashmovement = new Vector2(1.0f, dashmovement.y / dashmovement.x);
            }
            else
            {
                dashmovement = new Vector2(-1.0f, dashmovement.y / dashmovement.x);
            }
        }
        else if (dashmovement.x < dashmovement.y)
        {
            if (dashmovement.y > 0f)
            {
                dashmovement = new Vector2(dashmovement.x / dashmovement.y, 1.0f);
            }
            else
            {
                dashmovement = new Vector2(dashmovement.x / dashmovement.y, -1.0f);
            }
        }
        else
        {
            if (dashmovement.x > 0f)
            {
                if (dashmovement.y > 0f)
                {
                    dashmovement = new Vector2(1.0f, 1.0f);
                }
                else
                {
                    dashmovement = new Vector2(1.0f, -1.0f);
                }
            }
            else
            {
                if (dashmovement.y > 0f)
                {
                    dashmovement = new Vector2(-1.0f, 1.0f);
                }
                else
                {
                    dashmovement = new Vector2(-1.0f, -1.0f);
                }
            }
        }
        */
        //Debug.Log(dashmovement);

        rb.MovePosition(rb.position + dashmovement * speed * dashspeed * Time.fixedDeltaTime * 3);

        //rb.MovePosition(rb.position + dashmovement * speed * Time.fixedDeltaTime)
    }

    void StopDash()
    {
        isDashing = false;
        GameObject endWarp = Instantiate(handler.warpObj, transform.position, Quaternion.identity);

        /*
        float x = (endWarp.transform.position.x - currentDashObj.transform.position.x) / 3f;
        float y = (endWarp.transform.position.y - currentDashObj.transform.position.y) / 1.4f;

        if (x < 0)
        {
            x = x * -1f;
        }
        if (y < 0)
        {
            y = y * -1f;
        }

        if (x > 0.5f && y < 1f)
        {
            y = 1f;
        }
        else if (y > 0.5f && x < 1f)
        {
            x = 1f;
        }
        x = 0f;
        y = 0f;
        Debug.Log(currentDashObj.GetComponent<SpriteRenderer>().bounds.size);

        currentDashObj.transform.localScale = new Vector3(x, y);
        */
    }

    void TriggerDashFadeIn()
    {
        dashFadingOut = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            enemyCols += 1;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (!isDashing)
            {
                if (enemyCols > 0)
                {
                    enemyCols -= 1;
                }
            }
        }
        else if (collision.gameObject.tag == "DashableWall")
        {
            if (!isDashing)
            {
                if (enemyCols > 0)
                {
                    enemyCols -= 1;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "DashableWall")// || collision.gameObject.tag == "Crate") //To dash through crates
        {
            touchingDashWall = true;
        }
        else
        {
            touchingDashWall = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Cred")
        {
            Sound.PlaySound("coinpickup");
            handler.money += 1;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "Health")
        {
            handler.health += handler.maxHealth * 0.1f;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Shop")
        {
            handler.shopInRange = true;
        }
        else if (collision.gameObject.tag == "TP")
        {
            if (!collision.gameObject.GetComponent<Teleporter>().locked)
            {
                moveDisabled = true;
                isShooting = false;
                transform.position = new Vector3(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y + 0.2f);
                tpPos = collision.gameObject.GetComponent<Teleporter>().pos;

                if (collision.gameObject.GetComponent<Teleporter>().nextTeleporter.gameObject.name == "GameHandler")
                {
                    handler.settings.GetComponent<Settings>().gameCompleted = true;
                    handler.gameFinished = true;
                }

                nextTpLevel = collision.gameObject.GetComponent<Teleporter>().level + 1;

                handler.tpParticles.transform.position = transform.position;
                handler.tpParticles.GetComponent<SpriteRenderer>().enabled = true;

                Invoke("BeginTeleport", 1f);
                Invoke("Teleport", 5f);
                Invoke("TeleportFade", 3f);

                movement = new Vector2(0f, -1f);
                movement = new Vector2(0f, 0f);
            }
        }
        else if (collision.gameObject.name == "EnemyExplosion(Clone)")
        {
            handler.health -= collision.GetComponent<Explosion>().damage;

            /*
            GameObject popup = Instantiate(handler.dmgPopUp, transform.position, Quaternion.identity);
            popup.GetComponent<TextPopUp>().text = ((int)collision.GetComponent<Explosion>().damage).ToString();
            popup.GetComponent<TextPopUp>().target = gameObject;
            popup.GetComponent<TextPopUp>().damage = (int)collision.GetComponent<Explosion>().damage;
            */
        }
        else if (collision.gameObject.name == "MeleeBossBigSword(Clone)")
        {
            if (!handler.ultTransition && !handler.player.GetComponent<PlayerMove>().isDashing)
            {
                //Instantiate(handler.slashPE, collision.transform.position, Quaternion.identity);

                Vector3 new_pos = transform.position;

                Vector3 vec = new_pos - handler.player.transform.position;
                float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;

                Instantiate(handler.hitIndicator, handler.player.transform.position, Quaternion.AngleAxis(angle + 90, Vector3.forward));

                handler.playerHitTimer = 0;
                collision.gameObject.GetComponent<PlayerMove>().handler.health -= collision.GetComponent<Projectile>().damage;
            }
        }
        else if (collision.gameObject.tag == "Wall")
        {
            cancelDash = true;
            rb.isKinematic = false;
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shop")
        {
            handler.shopInRange = false;
        }
        if (collision.gameObject.tag == "Enemy")
        {
            if (enemyCols > 0)
            {
                enemyCols -= 1;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Poison")
        {
            handler.health -= 2;
            handler.playerHitTimer = 0;
        }
        else if (collision.gameObject.name == "Fire")
        {
            handler.health -= 5;
            handler.playerHitTimer = 0;
        }
    }

    //Hair Movement
    public void SetAnimX(float hfx)
    {
        hair.transform.localPosition = new Vector3(hfx, hair.transform.localPosition.y);
    }
    public void SetAnimY(float hfy)
    {
        hair.transform.localPosition = new Vector3(hair.transform.localPosition.x, hfy);
    }
    //Chestplate Movement
    public void SetAnimX2(float hfx)
    {
        chest.transform.localPosition = new Vector3(hfx, chest.transform.localPosition.y);
    }
    public void SetAnimY2(float hfy)
    {
        chest.transform.localPosition = new Vector3(chest.transform.localPosition.x, hfy);
    }
    //Shoulder Movement
    public void SetAnimX3(float hfx)
    {
        shoulder.transform.localPosition = new Vector3(hfx, shoulder.transform.localPosition.y);
    }
    public void SetAnimY3(float hfy)
    {
        shoulder.transform.localPosition = new Vector3(shoulder.transform.localPosition.x, hfy);
    }
    //Gun Movement
    public void SetAnimX4(float hfx)
    {
        gun.transform.localPosition = new Vector3(hfx, gun.transform.localPosition.y);
    }
    public void SetAnimY4(float hfy)
    {
        gun.transform.localPosition = new Vector3(gun.transform.localPosition.x, hfy);
    }

    void BeginTeleport()
    {
        isTeleporting = true;
        Sound.PlaySound("teleport");
    }
    void Teleport()
    {
        isTeleporting = false;
        transform.position = tpPos;

        handler.tpParticles.transform.position = transform.position;

        handler.health = handler.maxHealth;

        tpAlpha = 1f;
        Color x = transform.GetChild(6).GetComponent<SpriteRenderer>().color;
        x.a = 0.208f;
        transform.GetChild(6).GetComponent<SpriteRenderer>().color = x;

        brother.transform.position = new Vector3(transform.position.x - 2f, transform.position.y + 2f);
        brother.GetComponent<SoulFollow>().stopped = true;

        Invoke("EnableBrotherAfterTP", 3f);
    }

    void EnableBrotherAfterTP()
    {
        brother.GetComponent<SoulFollow>().stopped = false;

        moveDisabled = false;
        isShooting = true;
    }

    void TeleportFade()
    {
        handler.faderobj.GetComponent<Fade>().Fader(2f, 0.5f);
        Invoke("DisableTeleporter", 2f);

        if (handler.gameFinished)
        {
            Invoke("EndGame", 2f);
        }
    }
    void DisableTeleporter()
    {
        handler.settings.GetComponent<Settings>().currentLevel = nextTpLevel;
        handler.killed = 0;

        handler.bossHB.GetComponent<BossHealthBar>().ResetColor();

        if (handler.settings.GetComponent<Settings>().maxLevel < nextTpLevel)
        {
            handler.settings.GetComponent<Settings>().maxLevel = nextTpLevel;
        }
    }
    void EnableGun()
    {
        gun.GetComponent<SpriteRenderer>().enabled = true;
    }
    
    void EndGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SetCurrentNode(GameObject node)
    {
        currentNode = node;
    }
    public void Kill()
    {
        Destroy(gameObject);
    }

}
