using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    private GameHandler handler;

    private GameObject background;
    private GameObject healthBar;
    private GameObject bossIcon;
    //private GameObject bossWeapon;

    public GameObject boss;

    private bool colourChanged;
    private Texture2D originalTexture;
    private Image img;

    public float rotationSpeed;
    public float maxRotation;
    private float bossIconRotation;
    private bool rotatingLeft;

    private bool bossDead;
    private float alpha;

    // Start is called before the first frame update
    void Start()
    {
        handler = GameObject.Find("GameHandler").GetComponent<GameHandler>();

        colourChanged = false;
        rotatingLeft = true;

        bossDead = false;
        alpha = 0f;

        background = transform.GetChild(0).gameObject;
        healthBar = transform.GetChild(1).gameObject;
        bossIcon = transform.GetChild(2).gameObject;
        //bossWeapon = bossIcon.transform.GetChild(0).gameObject;

        img = bossIcon.GetComponent<Image>();
        originalTexture = img.sprite.texture;
    }

    // Update is called once per frame
    void Update()
    {
        boss = handler.bosses[handler.currentLevel - 1];

        if (boss != null)
        {
            healthBar.GetComponent<Image>().fillAmount = 0.27f + (0.7f * (boss.GetComponent<Enemy>().health / boss.GetComponent<Enemy>().originalhealth));
        }
        else
        {
            if (!bossDead)
            {
                bossDead = true;
                InvokeRepeating("FadeOut", 0f, 0.01f);
            }
        }

        if (rotatingLeft)
        {
            if (bossIconRotation < maxRotation - rotationSpeed)
            {
                bossIconRotation += rotationSpeed;
            }
            else
            {
                rotatingLeft = false;
            }
        }
        else
        {
            if (bossIconRotation > -maxRotation + rotationSpeed)
            {
                bossIconRotation -= rotationSpeed;
            }
            else
            {
                rotatingLeft = true;
            }
        }

        bossIcon.transform.rotation = new Quaternion(bossIcon.transform.rotation.x, bossIcon.transform.rotation.y, bossIconRotation, bossIcon.transform.rotation.w);

        //Debug.Log(colourChanged);

    }

    private void LateUpdate()
    {
        if (!colourChanged && boss != null)
        {
            //bossWeapon.GetComponent<Image>().color = boss.transform.GetChild(1).GetComponent<SpriteRenderer>().color;
            //bossWeapon.GetComponent<Image>().sprite = boss.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
            //bossWeapon.GetComponent<RectTransform>().sizeDelta = new Vector2(boss.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite.rect.width * 10, boss.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite.rect.height * 10);
            //bossWeapon.transform.position = bossIcon.transform.GetChild(1).transform.position;

            Texture2D copiedTexture = img.sprite.texture;
            Texture2D texture = new Texture2D(copiedTexture.width, copiedTexture.height);

            texture.filterMode = FilterMode.Point;

            for (int x = 0; x < copiedTexture.width; x++)
            {
                for (int y = 0; y < copiedTexture.height; y++)
                {
                    if (originalTexture.GetPixel(x, y) == boss.GetComponent<Enemy>().colors[0]) //Eyes
                    {
                        texture.SetPixel(x, y, boss.GetComponent<Enemy>().originalEyeColor);
                    }
                    else if (originalTexture.GetPixel(x, y) == boss.GetComponent<Enemy>().colors[1]) //Body
                    {
                        texture.SetPixel(x, y, boss.GetComponent<Enemy>().bodyColor);
                    }
                    else if (originalTexture.GetPixel(x, y) == boss.GetComponent<Enemy>().colors[2]) //Body Shadows
                    {
                        texture.SetPixel(x, y, boss.GetComponent<Enemy>().body2Color);
                    }
                    else if (originalTexture.GetPixel(x, y) == boss.GetComponent<Enemy>().colors[3]) //Head
                    {
                        texture.SetPixel(x, y, boss.GetComponent<Enemy>().headColor);
                    }
                    else if (originalTexture.GetPixel(x, y) == boss.GetComponent<Enemy>().colors[4]) //Headband
                    {
                        texture.SetPixel(x, y, boss.GetComponent<Enemy>().head2Color);
                    }
                    else
                    {
                        texture.SetPixel(x, y, originalTexture.GetPixel(x, y));
                    }
                }
            }

            texture.Apply();

            Sprite pixelSprite = Sprite.Create(texture, img.sprite.rect, new Vector2(0.5f, 0.5f), 16);
            img.sprite = pixelSprite;

            colourChanged = true;
        }
    }

    public void ResetColor()
    {
        colourChanged = false;
        //Debug.Log("RESET");
    }

    public void StartFadeIn()
    {
        InvokeRepeating("FadeIn", 0f, 0.01f);
    }

    private void FadeOut()
    {
        if (alpha >= 0.005f)
        {
            alpha -= 0.005f;
        }
        else
        {
            alpha = 0f;

            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alpha);
            background.GetComponent<Image>().color = new Color(background.GetComponent<Image>().color.r, background.GetComponent<Image>().color.g, background.GetComponent<Image>().color.b, alpha);
            healthBar.GetComponent<Image>().color = new Color(healthBar.GetComponent<Image>().color.r, healthBar.GetComponent<Image>().color.g, healthBar.GetComponent<Image>().color.b, alpha);
            bossIcon.GetComponent<Image>().color = new Color(bossIcon.GetComponent<Image>().color.r, bossIcon.GetComponent<Image>().color.g, bossIcon.GetComponent<Image>().color.b, alpha);

            CancelInvoke("FadeOut");

            bossDead = false;
        }

        GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alpha);
        background.GetComponent<Image>().color = new Color(background.GetComponent<Image>().color.r, background.GetComponent<Image>().color.g, background.GetComponent<Image>().color.b, alpha);
        healthBar.GetComponent<Image>().color = new Color(healthBar.GetComponent<Image>().color.r, healthBar.GetComponent<Image>().color.g, healthBar.GetComponent<Image>().color.b, alpha);
        bossIcon.GetComponent<Image>().color = new Color(bossIcon.GetComponent<Image>().color.r, bossIcon.GetComponent<Image>().color.g, bossIcon.GetComponent<Image>().color.b, alpha);

    }

    private void FadeIn()
    {
        if (alpha <= 1f - 0.005f)
        {
            alpha += 0.005f;
        }
        else
        {
            alpha = 1f;

            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alpha);
            background.GetComponent<Image>().color = new Color(background.GetComponent<Image>().color.r, background.GetComponent<Image>().color.g, background.GetComponent<Image>().color.b, alpha);
            healthBar.GetComponent<Image>().color = new Color(healthBar.GetComponent<Image>().color.r, healthBar.GetComponent<Image>().color.g, healthBar.GetComponent<Image>().color.b, alpha);
            bossIcon.GetComponent<Image>().color = new Color(bossIcon.GetComponent<Image>().color.r, bossIcon.GetComponent<Image>().color.g, bossIcon.GetComponent<Image>().color.b, alpha);

            CancelInvoke("FadeIn");
        }

        GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alpha);
        background.GetComponent<Image>().color = new Color(background.GetComponent<Image>().color.r, background.GetComponent<Image>().color.g, background.GetComponent<Image>().color.b, alpha);
        healthBar.GetComponent<Image>().color = new Color(healthBar.GetComponent<Image>().color.r, healthBar.GetComponent<Image>().color.g, healthBar.GetComponent<Image>().color.b, alpha);
        bossIcon.GetComponent<Image>().color = new Color(bossIcon.GetComponent<Image>().color.r, bossIcon.GetComponent<Image>().color.g, bossIcon.GetComponent<Image>().color.b, alpha);

    }
}
