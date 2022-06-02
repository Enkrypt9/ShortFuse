using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    [SerializeField] GameObject enemyHealthBar;
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
            bool hasHB = false;

            foreach(Transform child in collision.transform)
            {
                if (child.name == "EnemyHealthBar(Clone)")
                {
                    hasHB = true;
                    child.GetComponent<EnemyHB>().disappearTimer = 0;
                }
            }

            if (!hasHB && (collision.gameObject.GetComponent<Rigidbody2D>() != null))
            {
                GameObject HB = Instantiate(enemyHealthBar, new Vector3(collision.transform.position.x, collision.transform.position.y - (1f * collision.gameObject.transform.localScale.x)), Quaternion.identity);
                HB.transform.localScale = collision.transform.localScale;
                HB.GetComponent<EnemyHB>().anchor = collision.gameObject;

                HB.transform.SetParent(collision.gameObject.transform);
            }  
        }
    }
}
