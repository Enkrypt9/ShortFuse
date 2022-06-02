using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathColour : MonoBehaviour
{
    public Color[] colors;

    public Color eyeColor;
    public Color bodyColor;
    public Color body2Color;
    public Color headColor;
    public Color head2Color;


    // Update is called once per frame
    void LateUpdate()
    {
        Texture2D copiedTexture = GetComponent<SpriteRenderer>().sprite.texture;
        Texture2D texture = new Texture2D(copiedTexture.width, copiedTexture.height);

        texture.filterMode = FilterMode.Point;

        for (int x = 0; x < copiedTexture.width; x++)
        {
            for (int y = 0; y < copiedTexture.height; y++)
            {
                if (copiedTexture.GetPixel(x, y) == colors[0]) //Eyes
                {
                    texture.SetPixel(x, y, eyeColor);
                }
                else if (copiedTexture.GetPixel(x, y) == colors[1]) //Body
                {
                    texture.SetPixel(x, y, bodyColor);
                }
                else if (copiedTexture.GetPixel(x, y) == colors[2]) //Body Shadows
                {
                    texture.SetPixel(x, y, body2Color);
                }
                else if (copiedTexture.GetPixel(x, y) == colors[3]) //Head
                {
                    texture.SetPixel(x, y, headColor);
                }
                else if (copiedTexture.GetPixel(x, y) == colors[4]) //Headband
                {
                    texture.SetPixel(x, y, head2Color);
                }
                else
                {
                    texture.SetPixel(x, y, copiedTexture.GetPixel(x, y));
                }
            }
        }

        texture.Apply();

        Sprite pixelSprite = Sprite.Create(texture, GetComponent<SpriteRenderer>().sprite.rect, new Vector2(0.5f, 0.5f), 16);
        GetComponent<SpriteRenderer>().sprite = pixelSprite;
    }
}
