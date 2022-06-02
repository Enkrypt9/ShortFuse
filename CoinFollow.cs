using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFollow : MonoBehaviour
{
    private GameHandler handler;
    private GameObject player;
    private Vector2 direction;
    private Rigidbody2D rb;

    private float moveTime;
    private float moveTimer;

    public float speed;

    private Vector2 distance;
    public float stopDist;
    

    // Start is called before the first frame update
    void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        player = handler.player;
        rb = GetComponent<Rigidbody2D>();

        moveTime = Random.Range(0.5f, 1.2f);
        moveTimer = moveTime;
    }

    // Update is called once per frame
    void Update()
    {
        distance.x = player.transform.position.x - transform.position.x;
        distance.y = player.transform.position.y - transform.position.y;

        if ((-stopDist < distance.x && distance.x < stopDist) && (-stopDist < distance.y && distance.y < stopDist))
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0)
            {
                moveTimer = 0;
                direction = (player.transform.position - transform.position).normalized;
                rb.velocity = new Vector2(direction.x, direction.y) * speed;
            }
        }
    }
}
