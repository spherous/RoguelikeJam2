using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CardInputHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private CardDisplay cardDisplay;
    private CardDisplay zoomedCard;
    private GameObject cardPosition;
    public float offset;
    public bool dragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        Destroy(zoomedCard.gameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        cardDisplay.PlayCard();
    }

    public void OnDrag(PointerEventData eventData)
    {
        cardDisplay.transform.position = eventData.position;
        cardDisplay.transform.rotation = Quaternion.identity;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!dragging)
        {
            zoomedCard = Instantiate(cardDisplay, Vector3.zero, Quaternion.identity, cardPosition.transform);
            zoomedCard.transform.localPosition = new Vector3(cardDisplay.transform.localPosition.x, zoomedCard.transform.localPosition.y+offset, cardDisplay.transform.localPosition.z);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(zoomedCard != null)
            Destroy(zoomedCard.gameObject);
    }

    void Start() 
    {
        cardPosition = GameObject.Find("Card Position");    
    }
}