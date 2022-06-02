using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class NodeChild : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().SetCurrentNode(gameObject);
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (collision.gameObject.tag == "Player")
        {
            collision.GetComponent<PlayerMove>().SetCurrentNode(gameObject);
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (collision.gameObject.tag == "Player")
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
*/