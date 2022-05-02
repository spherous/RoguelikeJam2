using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public CardSO card;
    private GameObject gameManager;
    private GameObject zoomedCard;
    private CardSpawner cardSpawner;
    [SerializeField] GameObject parent;
    private GameObject cardPosition;
    public float offset;
    public bool dragging;
    public int threadCost;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        Destroy(zoomedCard);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;

        if(card != null && card.TryPlay())
        {
            cardSpawner.cardList.Remove(parent);
            cardSpawner.cardCount--;
            cardSpawner.SortCards();
            Destroy(parent);
            return;
        }

        cardSpawner.SortCards();
    }

    public void OnDrag(PointerEventData eventData)
    {
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
        threadCost = parent.GetComponent<CardDisplay>().card.threadCost;
        // threadPool = GameObject.FindObjectOfType<ThreadPool>();
        cardSpawner = gameManager.GetComponent<CardSpawner>();;
    }


}
