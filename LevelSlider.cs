using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSlider : MonoBehaviour
{
    [SerializeField] bool mouseDown;
    [SerializeField] float pivotPoint;
    private float previousCurrentValue;
    [SerializeField] float dragSpeed;

    [SerializeField] List<Button> buttons;
    [SerializeField] List<Button> zoomButtons;

    private float throwVelocity = 0f;
    private bool thrownRight = true;

    [SerializeField] List<float> buttonValues; //Each button has a value (an index) which is used to see where they lie in the size + pos array
    [SerializeField] List<float> sizeValues;
    [SerializeField] List<float> posValues;

    private float currentValue = 9f;
    private int currentInt = 9; //Level to slide to
    [SerializeField] float throwMultiplier; //How much it slides after release
    [SerializeField] float reboundMultiplier;

    private bool rebounding = false;
    private bool rebounded = true;

    [SerializeField] float idleTime; //How long does player have to hold down mouse to not slide
    private float idleTimer;

    //Remove
    public TextMeshProUGUI sliderStats;

    float posMultiplier = Screen.width / 1920f;

    // Start is called before the first frame update
    void Start()
    {
        idleTimer = idleTime;

        for(int i = 0; i < 19; i++)
        {
            posValues[i] = posValues[i] * posMultiplier;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float newposMultiplier = Screen.width / 1920f;

        if (newposMultiplier != posMultiplier)
        {
            for (int i = 0; i < 19; i++)
            {
                posValues[i] = posValues[i] / posMultiplier * newposMultiplier;
            }
            posMultiplier = newposMultiplier;
        }

        foreach (Button button in zoomButtons)
        {
            if (button.gameObject.transform.position.x != 0f)
            {
                button.gameObject.SetActive(true);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }

        sliderStats.text = "Current Value: " + currentValue + "\nVelocity: " + throwVelocity + "\nThrownRight: " + thrownRight + "\nRebounding: " + rebounding + "\nRebounded: " + rebounded + "\nIDLE: " + idleTimer;

        if (Input.GetMouseButtonDown(0)) //Original Click
        {
            previousCurrentValue = currentValue;
            pivotPoint = Input.mousePosition.x;
        }
        if (Input.GetMouseButton(0))
        {
            mouseDown = true;
        }
        else
        {
            mouseDown = false;
        }

        if (currentValue < 0f) //Stop dragging + sliding past level 1 and 10
        {
            currentValue = 0f;
        }
        else if (currentValue > 9f)
        {
            currentValue = 9f;
        }

        for (int i = 0; i < 10; i++) //Set level sizes + positions
        {
            buttonValues[i] = currentValue + i;
            float index = buttonValues[i];
            int intIndex = (int)index;
            float fraction = (index - intIndex) / (intIndex + 1 - intIndex);

            if (intIndex < 18)
            {
                float size = sizeValues[intIndex] + (fraction * (sizeValues[intIndex + 1] - sizeValues[intIndex]));
                float pos = posValues[intIndex] + (fraction * (posValues[intIndex + 1] - posValues[intIndex]));

                buttons[i].GetComponent<RectTransform>().position = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(pos + (960f * posMultiplier), 0f, 0f)).x, buttons[i].GetComponent<RectTransform>().position.y);
                buttons[i].GetComponent<RectTransform>().localScale = new Vector2(size, size);
            }
            else
            {
                buttons[i].GetComponent<RectTransform>().position = Vector2.zero;
                buttons[i].GetComponent<RectTransform>().localScale = Vector2.zero;
            }
        }

        if (mouseDown)
        {
            idleTimer -= Time.deltaTime;

            if (idleTimer > 0f)
            {
                float throwVec = (Input.mousePosition.x - pivotPoint) / 10000f;

                throwVelocity = throwVec / Time.deltaTime;

                if (throwVelocity > 0f)
                {
                    thrownRight = true;
                    if (throwVelocity > 0.5f)
                    {
                        throwVelocity = 0.5f;
                    }
                }
                else
                {
                    thrownRight = false;
                    if (throwVelocity < -0.5f)
                    {
                        throwVelocity = -0.5f;
                    }
                }
            }
            else
            {
                throwVelocity = 0f;
            }

            currentValue = previousCurrentValue + (Input.mousePosition.x - pivotPoint) / (1600 / dragSpeed);
        }
        else //If not dragging
        {
            idleTimer = idleTime;

            if (thrownRight && !rebounding)
            {
                if (throwVelocity > 0f)
                {
                    throwVelocity -= 0.01f;
                }
                else
                {
                    throwVelocity = 0f;
                }
            }
            else if (!thrownRight && !rebounding)
            {
                if (throwVelocity < 0f)
                {
                    throwVelocity += 0.01f;
                }
                else
                {
                    throwVelocity = 0f;
                }
            }

            if (throwVelocity == 0f && rebounding == false && rebounded == false) //If stopped sliding, not already called, and not in correct position
            {
                Rebound(Mathf.RoundToInt(currentValue));
            }
            else
            {
                if (currentValue != currentInt)
                {
                    rebounded = false;
                }
            }
            
            if (rebounding)
            {
                if (thrownRight && (currentValue >= currentInt))
                {
                    currentValue = currentInt;
                    throwVelocity = 0f;

                    rebounding = false;
                    rebounded = true;
                }
                else if (!thrownRight && (currentValue <= currentInt))
                {
                    currentValue = currentInt;
                    throwVelocity = 0f;

                    rebounding = false;
                    rebounded = true;
                }
            }

            currentValue += throwVelocity * throwMultiplier;
        }
    }

    void Rebound(int value)
    {
        currentInt = value;

        rebounding = true;
        rebounded = false;

        if (!thrownRight) //Swiped left
        {
            if (currentValue < currentInt) //Swiped under half (return to original)
            {
                throwVelocity = 1f * reboundMultiplier;
                thrownRight = true;
            }
            else if (currentValue > currentInt) //Swiped over half (jump to right)
            {
                throwVelocity = -1f * reboundMultiplier;
                thrownRight = false;
            }
        }
        else if (thrownRight) //Swiped right
        {
            if (currentValue < currentInt) //Swiped over half (jump to left)
            {
                throwVelocity = 1f * reboundMultiplier;
                thrownRight = true;
            }
            else if (currentValue > currentInt) //Swiped under half (return to original)
            {
                throwVelocity = -1f * reboundMultiplier;
                thrownRight = false;
            }
        }
    }
    public void Zoom(int value) //Same as rebound, only used when level that isn't in center is clicked, speed is faster
    {
        currentInt = value;

        rebounding = true;
        rebounded = false;

        if (!thrownRight) //Swiped left
        {
            if (currentValue < currentInt) //Swiped under half (return to original)
            {
                throwVelocity = 3f * reboundMultiplier;
                thrownRight = true;
            }
            else if (currentValue > currentInt) //Swiped over half (jump to right)
            {
                throwVelocity = -3f * reboundMultiplier;
                thrownRight = false;
            }
        }
        else if (thrownRight) //Swiped right
        {
            if (currentValue < currentInt) //Swiped over half (jump to left)
            {
                throwVelocity = 3f * reboundMultiplier;
                thrownRight = true;
            }
            else if (currentValue > currentInt) //Swiped under half (return to original)
            {
                throwVelocity = -3f * reboundMultiplier;
                thrownRight = false;
            }
        }
    }
    public void InitialZoom()
    {
        Zoom(10 - GetComponent<MenuHandler>().settings.GetComponent<Settings>().maxLevel);
    }
}
