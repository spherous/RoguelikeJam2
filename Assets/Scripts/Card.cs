using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private ThreadPool threadPool;
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
        

        if (threadCost > threadPool.availableThreads)
        {
            Debug.Log("Too expensive!");
            cardSpawner.SortCards();
        }
        else
        {
            Debug.Log("card played!");
            threadPool.Request(threadCost);
            cardSpawner.cardList.Remove(parent);
            cardSpawner.cardCount--;
            //if (cardSpawner.cardList.Count > 0)
            //{
                cardSpawner.SortCards();
            //}
            
            Destroy(parent);
        }

        

        
        

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
        threadPool = GameObject.FindObjectOfType<ThreadPool>();
        cardSpawner = gameManager.GetComponent<CardSpawner>();;
    }


}
