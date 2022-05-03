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
    private CardSpawner cardSpawner;

    void Start()
    {
        cardSpawner = GameObject.FindObjectOfType<CardSpawner>();
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
}