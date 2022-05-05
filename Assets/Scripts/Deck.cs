using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Deck : SerializedMonoBehaviour
{
    [SerializeField] private Hand hand;
    public List<ICard> cardsInDeck = new List<ICard>();
    public List<ICard> cardsInDiscard = new List<ICard>();
    public List<ICard> cardsInHand = new List<ICard>();
    public List<ICard> deletedCards = new List<ICard>();
    [OdinSerialize] public List<ICard> startingCards = new List<ICard>();
    void Awake()
    {
        for (int i = 0; i < startingCards.Count; i++)
        {
            cardsInDeck.Add(startingCards[i]);
        }
    }
    public void returnDiscard()
    {
        for (int i = 0; i < cardsInDiscard.Count; i++)
        {
            cardsInDeck.Add(cardsInDiscard[i]);
        }
        cardsInDiscard.Clear();
    }
    public void moveToDiscard(CardDisplay card)
    {
        cardsInHand.Remove(card.card);
        cardsInDiscard.Add(card.card);
    }
    public void moveToTrash(CardDisplay card)
    {
        cardsInHand.Remove(card.card);
        deletedCards.Add(card.card);
    }
    public void moveToHand(CardDisplay card)
    {
        cardsInDeck.Remove(card.card);
        cardsInHand.Add(card.card);
    }

}
