using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRadiusDetection : MonoBehaviour //Detects enemies + Crates
{
    private List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> alreadyHit = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            enemies.Add(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Crate")
        {
            enemies.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            enemies.Remove(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Crate")
        {
            enemies.Remove(collision.gameObject);
        }
    }
    public GameObject GetNearestEnemy(Vector3 startPoint)
    {
        float currentDistance = 0f;
        GameObject nearest  = null;

        if (enemies.Count > 0)
        {
            foreach (GameObject enemy in enemies)
            {
                if (!alreadyHit.Contains(enemy))
                {
                    float x = enemy.transform.position.x - startPoint.x;
                    float y = enemy.transform.position.y - startPoint.y;

                    float distance = Mathf.Sqrt(x * x + y * y);

                    if (nearest == null)
                    {
                        currentDistance = distance;
                        nearest = enemy;
                    }

                    if (distance < currentDistance)
                    {
                        nearest = enemy;
                    }
                }
            }

            if (nearest == null)
            {
                return null;
            }
            else
            {
                return nearest;
            }
        }
        else
        {
            return null;
        }
    }
}
