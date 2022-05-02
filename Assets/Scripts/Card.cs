using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject zoomedCard;
    private GameObject parent;
    private GameObject cardPosition;
    public float offset;

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        zoomedCard = Instantiate(parent, Vector3.zero, Quaternion.identity, cardPosition.transform);
        zoomedCard.transform.localPosition = new Vector3(parent.transform.localPosition.x, zoomedCard.transform.localPosition.y+offset, parent.transform.localPosition.z);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(zoomedCard);
    }

    void Start() 
    {
        //ugly hack to get the parent of the card
        cardPosition = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        parent = gameObject.transform.parent.gameObject;
    }
}
