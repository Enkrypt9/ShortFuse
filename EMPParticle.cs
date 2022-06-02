using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPParticle : MonoBehaviour
{
    private void OnParticleCollision(GameObject enemy)
    {
        if (enemy.tag == "Enemy")
        {
            enemy.GetComponent<Enemy>().Stun(4f, 1);
        }
    }
}
