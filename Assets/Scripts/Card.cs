using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private GameObject gameManager;
    private GameObject zoomedCard;
    [SerializeField] GameObject parent;
    private GameObject cardPosition;
    public float offset;
    public bool dragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("start dragging");
        dragging = true;
        Destroy(zoomedCard);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("end dragging");
        dragging = false;
        gameManager.GetComponent<CardSpawner>().SortCards();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("dragging");
        parent.transform.position = eventData.position;
        parent.transform.rotation = Quaternion.identity;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!dragging)
        {
            zoomedCard = Instantiate(parent, Vector3.zero, Quaternion.identity, cardPosition.transform);
            zoomedCard.transform.localPosition = new Vector3(parent.transform.localPosition.x, zoomedCard.transform.localPosition.y+offset, parent.transform.localPosition.z);
        }


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(zoomedCard);
    }

    void Start() 
    {
        cardPosition = GameObject.Find("Card Position");    
        gameManager = GameObject.Find("GameManager");
    }


}
