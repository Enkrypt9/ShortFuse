using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    private GameHandler handler;

    // Start is called before the first frame update
    void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = handler.player.transform.position;
    }
}
