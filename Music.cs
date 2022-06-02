using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public static AudioClip menu, game, end;
    static AudioSource audiosrcm;

    // Start is called before the first frame update
    void Start()
    {
        menu = Resources.Load<AudioClip>("MenuMusic");
        game = Resources.Load<AudioClip>("InGameMusic");
        end = Resources.Load<AudioClip>("EndMusic");
        audiosrcm = GetComponent<AudioSource>();
        audiosrcm.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void PlaySound(string clip)
    {
        if (clip == "menu")
        {
            audiosrcm.clip = menu;
            audiosrcm.Play();
        }
        else if (clip == "game")
        {
            audiosrcm.clip = game;
            audiosrcm.Play();
        }
        else if (clip == "end")
        {
            audiosrcm.clip = end;
            audiosrcm.Play();
        }
        else
        {
            Debug.Log("MUSIC CLIP DOES NOT EXIST: " + clip);
        }
    }
}
