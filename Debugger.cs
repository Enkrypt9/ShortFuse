using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    private List<float> pastFPS = new List<float>();
    private float totalFPS;
    private int FPScycles;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CalculateFPS", 0f, 1f);
    }

    void CalculateFPS()
    {
        pastFPS.Add((int)(1f / Time.unscaledDeltaTime));

        totalFPS = 0f;

        for (int i = 0; i < pastFPS.Count; i++)
        {
            totalFPS += pastFPS[i];
        }

        FPScycles += 1;

        Debug.Log("AVERAGE FPS AFTER " + FPScycles + " CYCLES: " + (totalFPS / FPScycles));

    }
}
