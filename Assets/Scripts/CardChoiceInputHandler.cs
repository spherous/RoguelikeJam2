using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardChoiceInputHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private CardDisplay cardDisplay;
    public Action clickAction;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        clickAction?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardDisplay.Outline(2);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardDisplay.Outline(0);
    }
}