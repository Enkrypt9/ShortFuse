using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int gCost;
    public int hCost;
    public int fCost;

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("COLLIDED with " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Crate" || collision.gameObject.tag == "Shop" || collision.gameObject.tag == "TP")
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("COLLIDED with " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Crate" || collision.gameObject.tag == "Shop" || collision.gameObject.tag == "TP")
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
