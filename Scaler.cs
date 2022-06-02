using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    private float originalPosX;
    private float originalPosY;
    private float originalScaleX;
    private float originalScaleY;
    private Vector2 deviceScreenResolution;
    private Vector2 pastScreenResolution;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            originalPosX = gameObject.GetComponent<RectTransform>().position.x;// - (Screen.width / 2);
            originalPosY = gameObject.GetComponent<RectTransform>().position.y;// - (Screen.height / 2);
        }
        catch
        {
            originalPosX = gameObject.GetComponent<Transform>().localPosition.x;
            originalPosY = gameObject.GetComponent<Transform>().localPosition.y;
        }

        originalScaleX = gameObject.transform.localScale.x;
        originalScaleY = gameObject.transform.localScale.y;

        //Debug.Log("ORIGINAL X:" + originalPosX);
        //Debug.Log("ORIGINAL Y:" + originalPosY);

        deviceScreenResolution = new Vector2(Screen.width, Screen.height);
        pastScreenResolution = deviceScreenResolution;

        //Debug.Log(deviceScreenResolution);

        Scale();
    }

    void Update()
    {
        deviceScreenResolution = new Vector2(Screen.width, Screen.height);

        if (pastScreenResolution != deviceScreenResolution)
        {
            Scale();

            pastScreenResolution = deviceScreenResolution;
        }

    }

    void Scale()
    {
        Camera.main.aspect = 16f / 9f;
        float srcWidth = Screen.width;
        float srcHeight = Screen.height;

        //float deviceScreenAspect = srcWidth / srcHeight;
        //Debug.Log("DEVICE ASPECT:" + deviceScreenAspect);

        float Xratio = (originalPosX / 960) * (srcWidth / 2);
        float Yratio = (originalPosY / 540) * (srcHeight / 2);

        //Debug.Log("XRATIO: " + Xratio);

        float aspect = srcWidth / 1920;

        //Debug.Log("ASPECT: " + aspect);
        //Debug.Log(deviceScreenResolution);

        try
        {
            //gameObject.GetComponent<RectTransform>().localPosition = new Vector3(Xratio - srcWidth / 2, Yratio - srcHeight / 2);
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(originalScaleX * aspect, originalScaleY * aspect);
        }
        catch
        {
            gameObject.GetComponent<Transform>().localPosition = new Vector3(Xratio, Yratio, gameObject.GetComponent<Transform>().localPosition.z);
            gameObject.GetComponent<Transform>().localScale = new Vector3(originalScaleX * aspect, originalScaleY * aspect);
        }
    }
}
/*
Vector2 deviceScreenResolution = new Vector2(Screen.width, Screen.height);
Debug.Log(deviceScreenResolution);

float srcHeight = Screen.height;
float srcWidth = Screen.width;

float deviceScreenAspect = srcWidth / srcHeight;

float camHeight = 100.0f * Camera.main.orthographicSize * 2.0f;
float camWidth = camHeight * deviceScreenAspect;

SpriteRenderer bg = gameObject.GetComponent<SpriteRenderer>();
float bgH = bg.sprite.rect.height;
float bgW = bg.sprite.rect.width;

float bgScaleH = camHeight / bgH;
float bgScaleW = camWidth / bgW;

gameObject.transform.localScale = new Vector3(bgScaleW, bgScaleH, 1);
*/