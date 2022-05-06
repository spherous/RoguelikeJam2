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
    private Hand hand;
    private MouseData mouseData;

    public bool disableAutoOutline = false;

    void Start()
    {
        mouseData = GameObject.FindObjectOfType<MouseData>();
        hand = GameObject.FindObjectOfType<Hand>();
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
        if (card.GetType() == typeof(TowerCard))
        {
            TowerCard towerCard = (TowerCard)card;
            BuildMode buildMode = GameObject.FindObjectOfType<BuildMode>();
            buildMode.buildModeOn = true;
            buildMode.card = card;

            if(card.singleUse == true)
            {
                hand.TrashCard(this);
                Destroy(gameObject);
                return;
            }
            hand.RemoveCard(this);
            Destroy(gameObject);
            return;
        }
        if(card != null && mouseData != null && mouseData.hoveredTile != null && card.TryPlay(mouseData.hoveredTile))
        {
            if(card.singleUse == true)
            {
                hand.TrashCard(this);
                Destroy(gameObject);
                return;
            }
            hand.RemoveCard(this);
            Destroy(gameObject);
            return;
        }
        ReturnCard();
    }
    public void ReturnCard()
    {
        transform.SetSiblingIndex(hand.cardList.IndexOf(this));
        hand.FanCards();
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

        if(disableAutoOutline)
            return;

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