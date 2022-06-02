using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Texture2D crosshair;
    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        //QualitySettings.vSyncCount = 0;

        Cursor.SetCursor(crosshair, new Vector2(0, 0), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }
}
