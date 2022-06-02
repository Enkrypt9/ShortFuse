using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GelScript : MonoBehaviour
{
    private Collider2D container;

    private void Start()
    {
        transform.GetChild(0).GetComponent<Light2D>().color = GetComponent<SpriteRenderer>().color;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (container == null)
        {
            if (collision.gameObject.layer == 14)
            {
                container = collision.gameObject.GetComponent<Collider2D>();
                //Debug.Log("YES");
                Physics2D.IgnoreCollision(container, GetComponent<BoxCollider2D>(), true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GetComponent<BoxCollider2D>().isTrigger == true)
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
        }
        else if (collision == container)
        {
            //Debug.Log("YES 222222");
            Physics2D.IgnoreCollision(container, GetComponent<BoxCollider2D>(), false);
            GetComponent<PolygonCollider2D>().enabled = false;
            GetComponent<Rigidbody2D>().mass = 10;
            GetComponent<Rigidbody2D>().drag = 1.5f;
            GetComponent<Rigidbody2D>().gravityScale = 1f;

            Invoke("StopGravity", 0.25f);
        }
    }

    private void StopGravity()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0f;
    }
}
