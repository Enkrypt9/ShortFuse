using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GelRotation : MonoBehaviour
{
    private int angle = 0;

    // Start is called before the first frame update
    void Start()
    {
        angle = Random.Range(0, 360);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
