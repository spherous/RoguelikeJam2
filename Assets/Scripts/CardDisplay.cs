using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardDisplay : MonoBehaviour
{
    public ICard card {get; private set;}
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image artworkImage;
    [SerializeField] private Image outline;
    [SerializeField] private float playDistance;
    
    public bool isDragging;
    private GameObject cardPosition;
    private CardSpawner cardSpawner;

    void Start()
    {
        cardSpawner = GameObject.FindObjectOfType<CardSpawner>();
        cardPosition = GameObject.Find("Card Position");
        outline.color = Color.clear;
        playDistance = playDistance*playDistance;
    }

    public void SetCard(ICard newCard)
    {
        card = newCard;
        nameText.text = card.name;
        costText.text = $"{card.threadCost}";
        descriptionText.text = card.description;
        artworkImage.sprite = card.artwork;
    }

    public void PlayCard()
    {
        if(card != null && card.TryPlay())
        {
            cardSpawner.RemoveCard(this);
            Destroy(gameObject);
            return;
        }
        cardSpawner.FanCards();
    }
    public void ReturnCard()
    {
        transform.SetSiblingIndex(cardSpawner.cardList.IndexOf(this));
        cardSpawner.FanCards();
    }
    public void Outline(int colorState)
    {
        if(colorState == 0)
        {
            outline.color = Color.clear;
            return;
        }
        else if(colorState == 1)
        {
            outline.color = Color.white;
            return;
        }
        else if(colorState == 2)
        {
            outline.color = Color.green;
            return;
        }

    }
    void Update() 
    {
        Vector3 offset = cardPosition.transform.position - transform.position;
        float sqrLen = offset.sqrMagnitude;

        if (sqrLen > playDistance && isDragging)
        {
            Outline(2);
        }
        else if (sqrLen < playDistance && isDragging)
        {
            Outline(1);
        }
        else 
        {
            Outline(0);
        }
    }
}