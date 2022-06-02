using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoulFollow : MonoBehaviour
{
    private GameHandler handler;
    private GameObject player;

    public float speed;
    private float originalspeed;

    private Rigidbody2D rb;

    private Vector2 distance;

    public float stopDist;
    private float originalStopDist;

    public bool TwoKeys;
    public bool animSwitched;
    public int DiagSwapTime;
    private float DiagSwapTimer;

    public Sprite[] images;

    public Vector2 movement;
    private Vector3 direction;

    public bool moveDisabled;
    public bool isShooting;
    public bool stopped;

    private bool bobbingUp;
    private float bobValue;

    public bool reachedPlayer;
    private bool invokedReached;

    private bool notPlayed;

    public Vector3 ultDistance; //Distance from player when ultimate is activated;

    public Sprite[] crownImages;
    private bool hasCrown;

    void Start()
    {
        notPlayed = true;
        invokedReached = false;

        isShooting = false;

        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        player = handler.player;

        rb = GetComponent<Rigidbody2D>();
        stopped = true;
        originalspeed = speed;
        bobbingUp = true;
        bobValue = 0f;

        originalStopDist = stopDist;
        reachedPlayer = false;

        if (handler.settings.GetComponent<Settings>().foundCrown)
        {
            hasCrown = true;
        }
        else
        {
            hasCrown = false;
        }

        transform.position = new Vector3(player.transform.position.x - 2f, player.transform.position.y);
    }

    private void FixedUpdate()
    {
        distance.x = player.transform.position.x - transform.position.x;
        distance.y = player.transform.position.y - transform.position.y;

        if (distance.x < 0)
        {
            distance.x = distance.x * -1;
        }
        if (distance.y < 0)
        {
            distance.y = distance.y * -1;
        }

        Vector3 dir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (stopDist < distance.x || stopDist < distance.y)
        {
            if (stopDist * 5 < distance.x || stopDist * 5 < distance.y)
            {
                speed = originalspeed * 20f;
            }
            else if (stopDist * 2 < distance.x || stopDist * 2 < distance.y)
            {
                speed = originalspeed * 1.5f;
            }
            else
            {
                speed = originalspeed;
            }
            stopped = false;

            Vector3 fromTarget = Vector3.ClampMagnitude(-dir.normalized, stopDist);
            Vector3 stopPoint = player.transform.position + fromTarget;

            transform.position = Vector2.MoveTowards(transform.position, stopPoint, speed * Time.deltaTime);
        }
        else
        {
            stopped = true;
        }
    }
    void Update()
    {

        if (handler.ultActive)
        {
            stopDist = 0f;

            if (!handler.ultTransition)
            {
                stopDist = 0f;
                notPlayed = true;
            }
            else
            {
                if (!invokedReached)
                {
                    invokedReached = true;
                    Invoke("ReachTrue", 1f);
                }

                if (!hasCrown)
                {
                    GetComponent<SpriteRenderer>().sprite = images[2];
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = crownImages[2];
                }

                if (notPlayed)
                {
                    notPlayed = false;
                    Sound.PlaySound("ult");
                }
            }
        }
        else
        {
            stopDist = originalStopDist;
        }

        if (stopped)
        {

            if (!handler.ultActive)
            {
                if (!player.GetComponent<PlayerMove>().moveDisabled || player.GetComponent<PlayerMove>().isTeleporting)
                {
                    if (bobbingUp)
                    {
                        bobValue += 0.002f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.002f);

                        if (bobValue >= 0.3f)
                        {
                            bobbingUp = false;
                        }
                    }
                    else
                    {
                        bobValue -= 0.002f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.002f);

                        if (bobValue <= 0f)
                        {
                            bobbingUp = true;
                        }
                    }
                }
            }
            else
            {
                /*
                reachedPlayer = true;
                movement = new Vector2(0f, -1f);
                GetComponent<SpriteRenderer>().sprite = images[2];
                */
            }
        }
        else
        {
            //Vector3 direction = (player.transform.position - transform.position).normalized;
            //rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
            stopped = false;
        }

        if (player.GetComponent<PlayerMove>().isTeleporting)
        {
            stopped = true;
        }
        else
        {
            if (!handler.ultActive)
            {
                speed = originalspeed;
            }
            else
            {
                speed = 0;

                if (!reachedPlayer)
                {
                    float diffX = transform.position.x - player.transform.position.x;

                    bool keepFlying = true; //To check if brother has overflown player

                    if (ultDistance.x > 0)
                    {
                        if (diffX < 0)
                        {
                            keepFlying = false;
                        }
                    }
                    else
                    {
                        if (diffX > 0)
                        {
                            keepFlying = false;
                        }
                    }

                    if (keepFlying)
                    {
                        transform.position = new Vector3(transform.position.x - ultDistance.x / 40, transform.position.y - ultDistance.y / 40);
                    }
                    else
                    {
                        reachedPlayer = true;
                        transform.position = player.transform.position;
                    }
                }
                else
                {
                    transform.position = player.transform.position;
                }
            }
        }

        if ((!player.GetComponent<PlayerMove>().moveDisabled || player.GetComponent<PlayerMove>().isTeleporting) && !stopped)
        {
            direction = (player.transform.position - transform.position).normalized;
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
                            animSwitched = false;
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[0];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[0];
                            }
                        }
                        else
                        {
                            animSwitched = true;
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[1];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[1];
                            }

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
                            animSwitched = false;
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[1];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[1];
                            }
                        }
                        else
                        {
                            animSwitched = true;
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[2];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[2];
                            }
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
                            animSwitched = false;
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[2];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[2];
                            }
                        }
                        else
                        {
                            animSwitched = true;
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[3];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[3];
                            }
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
                            animSwitched = false;
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[3];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[3];
                            }
                        }
                        else
                        {
                            animSwitched = true;
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[0];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[0];
                            }
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

                if (!(movement.x == 0f && movement.y == 0f))
                {
                    //hand.transform.localPosition = new Vector3(movement.x * 0.6f, movement.y * 0.6f);
                }
                if (!TwoKeys)
                {
                    if (movement.x == 0 && movement.y == 0)
                    {
                        if (!hasCrown)
                        {
                            GetComponent<SpriteRenderer>().sprite = images[2];
                        }
                        else
                        {
                            GetComponent<SpriteRenderer>().sprite = crownImages[2];
                        }
                    }
                    else
                    {
                        if (movement.x == 1)
                        {
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[1];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[1];
                            }
                        }
                        else if (movement.x == -1)
                        {
                            if (!hasCrown)
                            {
                                GetComponent<SpriteRenderer>().sprite = images[3];
                            }
                            else
                            {
                                GetComponent<SpriteRenderer>().sprite = crownImages[3];
                            }
                        }
                        else
                        {
                            if (movement.y == 1)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[0];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[0];
                                }
                            }
                            else if (movement.y == -1)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[2];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[2];
                                }
                            }
                        }
                    }
                }
            }
            else //If you are shooting
            {

                Vector3 pos = handler.player.transform.position - transform.position;
                if (pos.x > 0) //If on the right
                {
                    if (pos.y > 0) //Upper right corner
                    {
                        if (pos.x > pos.y) //Right quadrant
                        {
                            if (!TwoKeys)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[1];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[1];
                                }
                            }
                        }
                        else //Top Quadrant
                        {
                            if (!TwoKeys)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[0];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[0];
                                }
                            }
                        }
                        if (TwoKeys)
                        {
                            if ((int)DiagSwapTimer % DiagSwapTime == 0)
                            {
                                if (animSwitched)
                                {
                                    animSwitched = false;
                                    if (!hasCrown)
                                    {
                                        GetComponent<SpriteRenderer>().sprite = images[1];
                                    }
                                    else
                                    {
                                        GetComponent<SpriteRenderer>().sprite = crownImages[1];
                                    }
                                }
                                else
                                {
                                    animSwitched = true;
                                    if (!hasCrown)
                                    {
                                        GetComponent<SpriteRenderer>().sprite = images[0];
                                    }
                                    else
                                    {
                                        GetComponent<SpriteRenderer>().sprite = crownImages[0];
                                    }
                                }
                            }
                        }
                    }
                    else //Bottom right corner
                    {
                        pos.y = -pos.y;
                        if (pos.x > pos.y) //Right quadrant
                        {
                            if (!TwoKeys)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[1];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[1];
                                }
                            }
                        }
                        else //Bottom Quadrant
                        {
                            if (!TwoKeys)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[2];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[2];
                                }
                            }
                        }
                        if (TwoKeys)
                        {
                            if ((int)DiagSwapTimer % DiagSwapTime == 0)
                            {
                                if (animSwitched)
                                {
                                    animSwitched = false;
                                    if (!hasCrown)
                                    {
                                        GetComponent<SpriteRenderer>().sprite = images[1];
                                    }
                                    else
                                    {
                                        GetComponent<SpriteRenderer>().sprite = crownImages[1];
                                    }
                                }
                                else
                                {
                                    animSwitched = true;
                                    if (!hasCrown)
                                    {
                                        GetComponent<SpriteRenderer>().sprite = images[2];
                                    }
                                    else
                                    {
                                        GetComponent<SpriteRenderer>().sprite = crownImages[2];
                                    }
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
                            if (!TwoKeys)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[3];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[3];
                                }
                            }
                        }
                        else //Top quadrant
                        {
                            if (!TwoKeys)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[0];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[0];
                                }
                            }
                        }
                        if (TwoKeys)
                        {
                            if ((int)DiagSwapTimer % DiagSwapTime == 0)
                            {
                                if (animSwitched)
                                {
                                    animSwitched = false;
                                    if (!hasCrown)
                                    {
                                        GetComponent<SpriteRenderer>().sprite = images[3];
                                    }
                                    else
                                    {
                                        GetComponent<SpriteRenderer>().sprite = crownImages[3];
                                    }
                                }
                                else
                                {
                                    animSwitched = true;
                                    if (!hasCrown)
                                    {
                                        GetComponent<SpriteRenderer>().sprite = images[0];
                                    }
                                    else
                                    {
                                        GetComponent<SpriteRenderer>().sprite = crownImages[0];
                                    }
                                }
                            }
                        }
                    }
                    else //Bottom left corner
                    {
                        if (pos.x < pos.y) //Left quadrant
                        {
                            if (!TwoKeys)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[3];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[3];
                                }
                            }
                        }
                        else //Bottom quadrant
                        {
                            if (!TwoKeys)
                            {
                                if (!hasCrown)
                                {
                                    GetComponent<SpriteRenderer>().sprite = images[2];
                                }
                                else
                                {
                                    GetComponent<SpriteRenderer>().sprite = crownImages[2];
                                }
                            }
                        }
                        if (TwoKeys)
                        {
                            if ((int)DiagSwapTimer % DiagSwapTime == 0)
                            {
                                if (animSwitched)
                                {
                                    animSwitched = false;
                                    if (!hasCrown)
                                    {
                                        GetComponent<SpriteRenderer>().sprite = images[3];
                                    }
                                    else
                                    {
                                        GetComponent<SpriteRenderer>().sprite = crownImages[3];
                                    }
                                }
                                else
                                {
                                    animSwitched = true;
                                    if (!hasCrown)
                                    {
                                        GetComponent<SpriteRenderer>().sprite = images[2];
                                    }
                                    else
                                    {
                                        GetComponent<SpriteRenderer>().sprite = crownImages[2];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (!hasCrown)
            {
                GetComponent<SpriteRenderer>().sprite = images[2];
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = crownImages[2];
            }
        }
        if (player.GetComponent<PlayerMove>().isTeleporting)
        {
            if (!hasCrown)
            {
                GetComponent<SpriteRenderer>().sprite = images[2];
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = crownImages[2];
            }
        }
    }
    void ReachTrue()
    {
        reachedPlayer = true;
        invokedReached = false;
    }

    public void ActivateCrown()
    {
        hasCrown = true;
    }
}
