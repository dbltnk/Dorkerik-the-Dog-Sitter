using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite defaultSprite;
    public Sprite hoverSprite;
    private Image imageComponent;

    // Start is called before the first frame update
    void Start()
    {
        imageComponent = GetComponent<Image>();
        imageComponent.sprite = defaultSprite;
    }

    // When the mouse enters the Image component
    public void OnPointerEnter(PointerEventData eventData)
    {
        imageComponent.sprite = hoverSprite;
    }

    // When the mouse exits the Image component
    public void OnPointerExit(PointerEventData eventData)
    {
        imageComponent.sprite = defaultSprite;
    }
}