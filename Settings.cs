using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    private static GameObject instance;

    public float volume;

    public int currentLevel;
    public int maxLevel;
    public int cells;
    public Vector3 destination;

    public Vector3 hairRGB;
    public Vector3 clothesRGB;
    public int currentGun;
    public bool[] gunBought;

    public bool gamePlayed;
    public bool gameCompleted;

    public Vector3[] levelPos;

    public int healthLevel;

    public bool[] easterEggs;
    public bool foundCrown;
    public bool foundKatana;

    // Start is called before the first frame update
    void Start()
    {
        gamePlayed = false;

        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = gameObject;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
