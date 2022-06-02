using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class MenuHandler : MonoBehaviour
{
    public GameObject fader;
    public GameObject creds;
    public GameObject settings;
    public GameObject storyline;
    public TextMeshProUGUI story;

    private int fadestage = 1;
    private float CredsAlpha = 0;
    public float CAInc;
    public float CredStayTime;
    public bool storyOn;

    public string storytext;
    public string storydisplay;

    private float storytimer;
    public float storyspeed;
    private int index;

    private int storystage;
    private bool levelClicked;
    private float storyAlpha;

    public TextMeshProUGUI volumeText;
    public Slider volumeSlider;

    private float musicTimer;

    public Texture2D crosshair_img;

    public string[] easterEggTitles;

    public List<GameObject> buttonBGs;
    public Sprite[] buttonBGSprites = new Sprite[2];
    [SerializeField] Sprite invisibleButton;

    public GameObject sheen;
    public Sprite idleSheen;

    [SerializeField] List<Button> animatedButtons;

    // Start is called before the first frame update
    void Start()
    {
        musicTimer = 0;

        fader.GetComponent<Fade>().alpha = 1f;
        settings = GameObject.Find("Settings");

        if (!settings.GetComponent<Settings>().gamePlayed)
        {
            settings.GetComponent<Settings>().volume = volumeSlider.value;
        }
        else
        {
            volumeSlider.value = settings.GetComponent<Settings>().volume;
        }
        AudioListener.volume = settings.GetComponent<Settings>().volume;

        storystage = 0;
        storyOn = false;
        index = 0;
        levelClicked = false;
        storyAlpha = 1f;

        storytext = "Message Sent:\n\n'Dearest brother, it has been 3 years since you passed, and everyday there is not a moment that I do not miss you.\n\n" +
            "Your dying wish was for me to continue your research and stop the Corpo from inhibiting human consciousness from ascending.\n\n" +
            "You died learning everything you could about their beacon while working for the Corpo.\n" +
            "They employ every tool possible to enslave the collective consciousness and they continue to do so even to those that have passed like you.\n\n" +
            "They use our physical energy to run their system - without us they are nothing. But now they use the energy of the spirit to power their " +
            "Super Servers; the hardware that serves the media to the population - consumers distracting themselves\nfrom their physical enslavement." +
            "They all choose blissful ignorance over the truth. After all, the truth is a bitter pill to swallow.\n\n" +
            "I don't know how you managed to connect to my physical interface, my guess, based on your research is that your consciousness travelled through their networks.\n" +
            "Your research details a backdoor in the beacon firewall. Of course you would know how to circumvent something like that, you always were better with tech.\n\n" +
            "Luckily I have my shooting skills - my time serving in the AI Revolt served me well. Now is the time to use all that training against the powers that used all of us to oppress harmless AI.\n\n" +
            "I know what I need to do. Destroy the beacon. Free the captured consciousness of those that were physically enslaved by the Corpo. They deserve to move on.'\n\n" +
            "...                                                     \n\n" +
            "Message Receieved:\n\n'Good Luck brother.'";

        if (settings.GetComponent<Settings>().gamePlayed)
        {
            fader.SetActive(false);
            creds.gameObject.SetActive(false);
            fadestage = 100;
        }

        Cursor.SetCursor(crosshair_img, new Vector2(0, 0), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.K)) //REMOVE THIS
        {
            if (Application.targetFrameRate == 60)
            {
                Application.targetFrameRate = 10;
            }
            else
            {
                Application.targetFrameRate = 60;
            }
        }
        */
        if (musicTimer >= 0)
        {
            musicTimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;
        }

        if (musicTimer >= 200)
        {
            PlayMusic();
            musicTimer = -100f;
        }

        settings.GetComponent<Settings>().volume = volumeSlider.value;
        volumeText.text = (Convert.ToInt32(volumeSlider.value * 200)) + "%";

        AudioListener.volume = settings.GetComponent<Settings>().volume;

        if (!settings.GetComponent<Settings>().gamePlayed)
        {
            if (fadestage == 1)
            {
                if (CredsAlpha < 1)
                {
                    CredsAlpha += CAInc * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }

                //Color x = creds.GetComponent<TextMeshProUGUI>().color;
                //x.a = CredsAlpha;
                //creds.GetComponent<TextMeshProUGUI>().color = x;

                foreach (Transform text in creds.transform)
                {
                    Color y = text.GetComponent<TextMeshProUGUI>().color;
                    y.a = CredsAlpha;
                    text.GetComponent<TextMeshProUGUI>().color = y;
                }

                if (CredsAlpha >= 1)
                {
                    fadestage = 0;
                    Invoke("BeginFadeOut", 2f);
                }

            }
            else if (fadestage == 2)
            {
                if (CredsAlpha > CAInc)
                {
                    CredsAlpha -= CAInc * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }

                //Color x = creds.GetComponent<TextMeshProUGUI>().color;
                //x.a = CredsAlpha;
                //creds.GetComponent<TextMeshProUGUI>().color = x;

                foreach (Transform text in creds.transform)
                {
                    Color y = text.GetComponent<TextMeshProUGUI>().color;
                    y.a = CredsAlpha;
                    text.GetComponent<TextMeshProUGUI>().color = y;
                }

                if (CredsAlpha <= CAInc)
                {
                    fadestage = 3;
                }
            }
            else if (fadestage == 3)
            {
                fader.GetComponent<Fade>().FaderOut(0.5f, 0.5f);
                Invoke("ActivateSheenPulse", 1.3f);
                Invoke("TurnFaderOff", 1.5f);
                creds.gameObject.SetActive(false);
                fadestage = 100;
                /*
                if (CredsAlpha < (1 - CAInc))
                {
                    CredsAlpha += CAInc;
                }
                else
                {
                    //fader.GetComponent<Fade>().alpha = 0f;
                    fadestage = 100;
                    //fader.SetActive(false);
                }
                Color x = fader.GetComponent<Image>().color;
                x.a = 1 - CredsAlpha;
                fader.GetComponent<Image>().color = x;
                */
            }
        }
        if (storyOn)
        {
            story.text = storydisplay;

            if (storystage == 0)
            {
                storytimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;

                if (index < storytext.Length)
                {
                    if ((int)storytimer % storyspeed == 0)
                    {
                        storydisplay += storytext[index];
                        index += 1;
                    }
                }
                else
                {
                    if (index == storytext.Length)
                    {
                        storystage += 1;
                        index += 1;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (storystage == 0)
                {
                    storydisplay = storytext;
                }
                storystage += 1;
            }
            if (storystage > 1)
            {
                if (storyAlpha > 0.01f)
                {
                    storyAlpha -= 0.01f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }
                else
                {
                    Invoke("StartGame", 0f);
                }

                foreach (Transform child in storyline.transform)
                {
                    Color x = child.GetComponent<TextMeshProUGUI>().color;
                    x.a = storyAlpha;
                    child.GetComponent<TextMeshProUGUI>().color = x;
                }
            }
        }
    }

    void FadeOut()
    {
        fadestage = 2;
    }

    public void PlayGame()
    {
        if (!levelClicked)
        {
            levelClicked = true;
            fader.SetActive(true);
            fader.GetComponent<Fade>().FaderIn(2f, 0.5f); //
            if (settings.GetComponent<Settings>().gamePlayed)
            {
                Invoke("StartGame", 2f);
            }
            else
            {
                Invoke("StoryOn", 2f);
            }
        }
    }
    void StartGame()
    {
        settings.GetComponent<Settings>().gamePlayed = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void BeginFadeOut()
    {
        Invoke("FadeOut", CredStayTime);
    }
    void StoryOn()
    {
        storyline.SetActive(true);
        storyOn = true;
    }
    void TurnFaderOff()
    {
        fader.SetActive(false);
    }
    public void ActivateSheenPulse()
    {
        sheen.GetComponent<Animator>().SetBool("ActivatePulse", true);
    }
    public void PlayClick()
    {
        Sound.PlaySound("click");
    }
    void PlayMusic()
    {
        Music.PlaySound("menu");
    }

    public void OpenLink(int LinkNumber)
    {
        if (LinkNumber == 1)
        {
            Application.OpenURL("https://www.instagram.com/troysart_/");
        }
        else if (LinkNumber == 2)
        {
            Application.OpenURL("https://www.instagram.com/enkrypt.ig/");
        }
        else if (LinkNumber == 3)
        {
            Application.OpenURL("https://www.instagram.com/heyvulp/");
        }
        else if (LinkNumber == 4)
        {
            Application.OpenURL("https://soundcloud.com/vulpmusic");
        }
        else if (LinkNumber == 5)
        {
            Application.OpenURL("https://www.youtube.com/channel/UCx66yD6ckAYRp6JHcm08-3w");
        }
        else if (LinkNumber == 6)
        {
            Application.OpenURL("https://enkrypt.itch.io/short-fuse/devlog/385184/update-12");
        }
    }

    public void Highlight(int buttonNumber) //0 = Start, 1 = Options, 2 = Creds, 3 = Changelog, 4 = LevelsBack, 5 = OptionsBack, 6 = CredsBack, 7 = ChangeBack, 8 = EasterEgg, 9 = EEBack
    {
        buttonBGs[buttonNumber].GetComponent<Image>().sprite = buttonBGSprites[1];
    }
    public void UnHighlight(int buttonNumber)
    {
        buttonBGs[buttonNumber].GetComponent<Image>().sprite = buttonBGSprites[0];
    }
    public void ResetAnimatedButtons()
    {
        foreach (GameObject background in buttonBGs)
        {
            background.GetComponent<Image>().sprite = buttonBGSprites[0];
        }
        foreach (Button button in animatedButtons)
        {
            button.GetComponent<Image>().sprite = invisibleButton;
        }
    }
}
