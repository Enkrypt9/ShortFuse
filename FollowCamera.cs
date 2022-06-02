using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private GameHandler handler;

    public bool followBoss;

    private GameObject currentBoss;
    private Vector3 bossPos;
    private float Xincrement;
    private float Yincrement;

    private int counter;

    public GameObject canvas;
    public GameObject cbar1;
    public GameObject cbar2;

    public bool cutsceneActive;

    // Start is called before the first frame update
    void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();

        cutsceneActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!followBoss)
        {
            transform.position = new Vector3(handler.player.transform.position.x, handler.player.transform.position.y, transform.position.z);
        }
    }

    public void StartBossMove(GameObject boss)
    {
        //Debug.Log("STARTED");

        cutsceneActive = true;

        handler.overlayUI.GetComponent<Overlay>().StopPulse();

        handler.PauseUltimate();

        handler.player.GetComponent<PlayerMove>().isShooting = false;
        handler.player.GetComponent<PlayerMove>().moveDisabled = true;
        handler.player.GetComponent<PlayerMove>().movement = new Vector2(0f, -1f);
        handler.player.GetComponent<PlayerMove>().movement = new Vector2(0f, 0f);

        followBoss = true;

        currentBoss = boss;
        bossPos = currentBoss.transform.position;

        Xincrement = (bossPos.x - handler.player.transform.position.x) / 200;
        Yincrement = (bossPos.y - handler.player.transform.position.y) / 200;

        InvokeRepeating("MoveTowardsBoss", 0f, 0.01f);

        foreach(Transform child in canvas.gameObject.transform.GetChild(1))
        {
            try
            {
                child.GetComponent<CanvasFader>().BeginFadeOut();
            }
            catch
            {
                ;
            }
        }
    }
    void MoveTowardsBoss()
    {
        cbar1.transform.position = new Vector3(cbar1.transform.position.x, cbar1.transform.position.y - 0.015f);
        cbar2.transform.position = new Vector3(cbar2.transform.position.x, cbar2.transform.position.y + 0.015f);

        counter += 1;

        transform.position = new Vector3(transform.position.x + Xincrement, transform.position.y + Yincrement, transform.position.z);

        if (counter >= 200)
        {
            counter = 0;

            CancelInvoke("MoveTowardsBoss");
        }
    }

    public void FinishBossMove(GameObject boss)
    {
        //Debug.Log("FINISHING");

        followBoss = true;

        bossPos = boss.transform.position;

        Xincrement = (bossPos.x - handler.player.transform.position.x) / 200;
        Yincrement = (bossPos.y - handler.player.transform.position.y) / 200;

        InvokeRepeating("MoveFromBoss", 0f, 0.01f);

        handler.bossHB.GetComponent<BossHealthBar>().StartFadeIn();

        foreach (Transform child in canvas.gameObject.transform.GetChild(1))
        {
            try
            {
                child.GetComponent<CanvasFader>().BeginFadeIn();
            }
            catch
            {
                ;
            }
        }
    }

    void MoveFromBoss()
    {
        cbar1.transform.position = new Vector3(cbar1.transform.position.x, cbar1.transform.position.y + 0.015f);
        cbar2.transform.position = new Vector3(cbar2.transform.position.x, cbar2.transform.position.y - 0.015f);

        counter += 1;

        transform.position = new Vector3(transform.position.x - Xincrement, transform.position.y - Yincrement, transform.position.z);

        if (counter >= 200)
        {
            counter = 0;

            Invoke("UndisableBoss", 1f);

            followBoss = false;

            handler.UnpauseUltimate();
            handler.player.GetComponent<PlayerMove>().moveDisabled = false;

            cutsceneActive = false;

            CancelInvoke("MoveFromBoss");
        }
    }

    void UndisableBoss()
    {
        currentBoss.GetComponent<Enemy>().Undisable();
    }
}
