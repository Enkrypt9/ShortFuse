using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CredsHandler : MonoBehaviour
{
    private bool storyOn;

    public TextMeshProUGUI story;
    public GameObject storyline;
    public GameObject credits;

    public string storytext;
    public string storydisplay;

    private float storytimer;
    public float storyspeed;
    private int index;

    private int storystage;
    private float storyAlpha;

    private float fadeinalpha;

    private float musicTimer;

    public Texture2D crosshair_img;

    // Start is called before the first frame update
    void Start()
    {
        storyOn = true;

        storystage = 0;
        index = 0;
        storyAlpha = 1f;
        fadeinalpha = 0f;

        storytext = "Message Sent:\n\n'Brother, we did it! We destroyed the beacon! Your soul can finally ascend, and you can be free!'\n\n" +
            "Message Receieved:\n\n'I know. You did well. However, on this journey, I thought long and hard, and I have decided that I wish to stay here with you.'\n\n" +
            "Message Sent:\n\n'As long as that is your only wish, then there are many more corporations that need to be put in their places!'\n\n" +
            "\n\nYou smile at your interface, knowing that your brother will stay by your side.\n\n" +
            "After all, you are stronger together.";

        Cursor.SetCursor(crosshair_img, new Vector2(0, 0), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        musicTimer += 1 * 60f / (1f / Time.deltaTime); //Change first number for rate;

        if (musicTimer >= 60)
        {
            PlayMusic();
            musicTimer = -100f;
        }

        if (storyOn)
        {
            story.text = storydisplay;

            if (storystage == 0)
            {
                if (fadeinalpha < 1f)
                {
                    fadeinalpha += 0.01f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }

                foreach (Transform child in storyline.transform)
                {
                    if (child.gameObject.name != "StoryText")
                    {
                        Color x = child.GetComponent<TextMeshProUGUI>().color;
                        x.a = fadeinalpha;
                        child.GetComponent<TextMeshProUGUI>().color = x;
                    }
                }

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
                if (storystage < 3)
                {
                    storystage += 1;
                }
            }
            if (storystage == 2)
            {
                if (storyAlpha > 0.01f)
                {
                    storyAlpha -= 0.01f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }
                else
                {
                    storystage += 1;
                }

                foreach (Transform child in storyline.transform)
                {
                    Color x = child.GetComponent<TextMeshProUGUI>().color;
                    x.a = storyAlpha;
                    child.GetComponent<TextMeshProUGUI>().color = x;
                }
            }
            else if (storystage == 3)
            {
                storyline.SetActive(false);
                credits.SetActive(true);

                if (storyAlpha < 1)
                {
                    storyAlpha += 0.01f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }
                else
                {
                    storystage += 1;
                    Invoke("Finish", 7f);
                }

                foreach (Transform child in credits.transform)
                {
                    Color x = child.GetComponent<TextMeshProUGUI>().color;
                    x.a = storyAlpha;
                    child.GetComponent<TextMeshProUGUI>().color = x;
                }
            }
            else if (storystage == 5)
            {
                if (storyAlpha > 0)
                {
                    storyAlpha -= 0.01f * 60f / (1f / Time.deltaTime); //Change first number for rate;
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
                }

                foreach (Transform child in credits.transform)
                {
                    Color x = child.GetComponent<TextMeshProUGUI>().color;
                    x.a = storyAlpha;
                    child.GetComponent<TextMeshProUGUI>().color = x;
                }
            }
        }
    }
    void Finish()
    {
        storystage += 1;
    }

    void PlayMusic()
    {
        Music.PlaySound("end");
    }
}
