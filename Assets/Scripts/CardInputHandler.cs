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
    private CardSpawner cardSpawner;
    public bool isDragging;
    public float offset;


    private void OnDestroy()
    {
        if(zoomedCard != null)
            Destroy(zoomedCard.gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        cardDisplay.isDragging = true;
        cardDisplay.transform.SetSiblingIndex(cardPosition.transform.childCount-1);
        if(zoomedCard != null)
            Destroy(zoomedCard.gameObject);
        
        cardDisplay.transform.SetSiblingIndex(cardPosition.transform.childCount - 1);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        cardDisplay.isDragging = false;
        if (Vector3.Distance(gameObject.transform.position, cardPosition.transform.position) < 1200)
        {
            cardDisplay.ReturnCard();
            return;

        }
        cardDisplay.PlayCard();
    }

    public void OnDrag(PointerEventData eventData)
    {
        cardDisplay.transform.position = eventData.position;
        cardDisplay.transform.rotation = Quaternion.identity;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isDragging)
        {
            zoomedCard = Instantiate(cardDisplay, cardPosition.transform);
            zoomedCard.transform.localPosition = new Vector3(cardDisplay.transform.localPosition.x, zoomedCard.transform.localPosition.y+offset, cardDisplay.transform.localPosition.z);
            zoomedCard.transform.rotation = Quaternion.identity;
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
        cardSpawner = GameObject.FindObjectOfType<CardSpawner>();
    }


    
}