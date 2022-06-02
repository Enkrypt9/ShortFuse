using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    float originalSize;
    Vector2 deviceScreenResolution;
    Vector2 pastScreenResolution;

    // Start is called before the first frame update
    void Start()
    {
        originalSize = GetComponent<Camera>().orthographicSize;

        deviceScreenResolution = new Vector2(Screen.width, Screen.height);
        pastScreenResolution = deviceScreenResolution;

        CameraScale();
    }

    // Update is called once per frame
    void Update()
    {
        deviceScreenResolution = new Vector2(Screen.width, Screen.height);

        if (pastScreenResolution != deviceScreenResolution)
        {
            CameraScale();

            pastScreenResolution = deviceScreenResolution;
        }
        Debug.Log(GetComponent<Camera>().orthographicSize);
    }

    void CameraScale()
    {
        float divisionX = deviceScreenResolution.x / 1920;
        float divisionY = deviceScreenResolution.y / 1080;

        float division = (divisionX + divisionY) / 2;

        GetComponent<Camera>().orthographicSize = originalSize / division;
    }
}
