using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Sound : MonoBehaviour
{
    public static AudioClip click, box1, box2, coinpickup, pistol1, pistol2, pistol3, pistol4, pistol5, machinegun, shotty1, shotty2, shotty3, shotty4, shotty5, wavegun, handcannon, launcher, robodeath1, robodeath2, robodeath3, robodeath4, buy, death, respawn, teleport, slash1, slash2, slash3, slash4, overheat, ult, explosion;
    static AudioSource audiosrcm;
    // Start is called before the first frame update
    void Start()
    {
        click = Resources.Load<AudioClip>("Menu Click");
        box1 = Resources.Load<AudioClip>("Box Break 01");
        box2 = Resources.Load<AudioClip>("Box Break 02");
        coinpickup = Resources.Load<AudioClip>("Coin Pickup");
        pistol1 = Resources.Load<AudioClip>("Pistol Shot 01");
        pistol2 = Resources.Load<AudioClip>("Pistol Shot 02");
        pistol3 = Resources.Load<AudioClip>("Pistol Shot 03");
        pistol4 = Resources.Load<AudioClip>("Pistol Shot 04");
        pistol5 = Resources.Load<AudioClip>("Pistol Shot 05");
        machinegun = Resources.Load<AudioClip>("Machine Gun Chop 2.0");
        shotty1 = Resources.Load<AudioClip>("Shotgun 01");
        shotty2 = Resources.Load<AudioClip>("Shotgun 02");
        shotty3 = Resources.Load<AudioClip>("Shotgun 03");
        shotty4 = Resources.Load<AudioClip>("Shotgun 04");
        shotty5 = Resources.Load<AudioClip>("Shotgun 05");
        wavegun = Resources.Load<AudioClip>("Wavegun");
        handcannon = Resources.Load<AudioClip>("Hand Cannon");
        launcher = Resources.Load<AudioClip>("Rocket Launch");
        robodeath1 = Resources.Load<AudioClip>("Robot Death 01");
        robodeath2 = Resources.Load<AudioClip>("Robot Death 02");
        robodeath3 = Resources.Load<AudioClip>("Robot Death 03");
        robodeath4 = Resources.Load<AudioClip>("Robot Death 04");
        buy = Resources.Load<AudioClip>("Shop Purchase");
        death = Resources.Load<AudioClip>("Death Wind Down");
        respawn = Resources.Load<AudioClip>("Revive Wind Up");
        teleport = Resources.Load<AudioClip>("Portal");
        slash1 = Resources.Load<AudioClip>("Sword_Hit_01");
        slash2 = Resources.Load<AudioClip>("Sword_Hit_02");
        slash3 = Resources.Load<AudioClip>("Sword_Hit_03");
        slash4 = Resources.Load<AudioClip>("Sword_Hit_04");
        overheat = Resources.Load<AudioClip>("Overheat");
        ult = Resources.Load<AudioClip>("Ulty_Charge");
        explosion = Resources.Load<AudioClip>("Explosion");

        audiosrcm = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void PlaySound(string clip)
    {
        if (clip == "click")
        {
            audiosrcm.PlayOneShot(click);
        }
        else if (clip == "box1")
        {
            audiosrcm.PlayOneShot(box1);
        }
        else if (clip == "box2")
        {
            audiosrcm.PlayOneShot(box2);
        }
        else if (clip == "coinpickup")
        {
            audiosrcm.PlayOneShot(coinpickup);
        }
        else if (clip == "pistol1")
        {
            audiosrcm.PlayOneShot(pistol1);
        }
        else if (clip == "pistol2")
        {
            audiosrcm.PlayOneShot(pistol2);
        }
        else if (clip == "pistol3")
        {
            audiosrcm.PlayOneShot(pistol3);
        }
        else if (clip == "pistol4")
        {
            audiosrcm.PlayOneShot(pistol4);
        }
        else if (clip == "pistol5")
        {
            audiosrcm.PlayOneShot(pistol5);
        }
        else if (clip == "machinegun")
        {
            audiosrcm.PlayOneShot(machinegun);
        }
        else if (clip == "shotty1")
        {
            audiosrcm.PlayOneShot(shotty1);
        }
        else if (clip == "shotty2")
        {
            audiosrcm.PlayOneShot(shotty2);
        }
        else if (clip == "shotty3")
        {
            audiosrcm.PlayOneShot(shotty3);
        }
        else if (clip == "shotty4")
        {
            audiosrcm.PlayOneShot(shotty4);
        }
        else if (clip == "shotty5")
        {
            audiosrcm.PlayOneShot(shotty5);
        }
        else if (clip == "wavegun")
        {
            audiosrcm.PlayOneShot(wavegun);
        }
        else if (clip == "handcannon")
        {
            audiosrcm.PlayOneShot(handcannon);
        }
        else if (clip == "launcher")
        {
            audiosrcm.PlayOneShot(launcher);
        }
        else if (clip == "robodeath1")
        {
            audiosrcm.PlayOneShot(robodeath1);
        }
        else if (clip == "robodeath2")
        {
            audiosrcm.PlayOneShot(robodeath2);
        }
        else if (clip == "robodeath3")
        {
            audiosrcm.PlayOneShot(robodeath3);
        }
        else if (clip == "robodeath4")
        {
            audiosrcm.PlayOneShot(robodeath4);
        }
        else if (clip == "buy")
        {
            audiosrcm.PlayOneShot(buy);
        }
        else if (clip == "death")
        {
            audiosrcm.PlayOneShot(death);
        }
        else if (clip == "respawn")
        {
            audiosrcm.PlayOneShot(respawn);
        }
        else if (clip == "teleport")
        {
            audiosrcm.PlayOneShot(teleport);
        }
        else if (clip == "slash1")
        {
            audiosrcm.PlayOneShot(slash1);
        }
        else if (clip == "slash2")
        {
            audiosrcm.PlayOneShot(slash2);
        }
        else if (clip == "slash3")
        {
            audiosrcm.PlayOneShot(slash3);
        }
        else if (clip == "slash4")
        {
            audiosrcm.PlayOneShot(slash4);
        }
        else if (clip == "overheat")
        {
            audiosrcm.PlayOneShot(overheat);
        }
        else if (clip == "ult")
        {
            audiosrcm.PlayOneShot(ult);
        }
        else if (clip == "explosion")
        {
            audiosrcm.PlayOneShot(explosion);
        }
    }
}
