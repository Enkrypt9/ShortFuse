using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MenuHandler handler;
    [SerializeField] int buttonNumber;

    private void Start()
    {
        handler = GameObject.Find("Handler").GetComponent<MenuHandler>(); 
    }
    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        handler.Highlight(buttonNumber);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        handler.UnHighlight(buttonNumber);
    }
}