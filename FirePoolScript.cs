using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePoolScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("FireDeath", GetComponent<Kill>().killTime - 4f);
    }

    void FireDeath()
    {
        transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        Destroy(transform.GetChild(0).gameObject, 2f);
    }
}
