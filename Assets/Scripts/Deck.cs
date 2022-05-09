using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class Deck : SerializedMonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    public List<ICard> cardsInDeck = new List<ICard>();
    public List<ICard> cardsInDiscard = new List<ICard>();
    public List<ICard> deletedCards = new List<ICard>();
    [OdinSerialize] public List<ICard> startingCards = new List<ICard>();
    void Awake()
    {
        for(int i = 0; i < startingCards.Count; i++)
            cardsInDeck.Add(startingCards[i]);
        
        levelManager.onLevelComplete += OnLevelComplete;
    }

    private void OnDestroy() => levelManager.onLevelComplete -= OnLevelComplete;

    private void OnLevelComplete(Level level) => ReturnDeleted();

    public void ReturnDiscard()
    {
        for(int i = 0; i < cardsInDiscard.Count; i++)
            cardsInDeck.Add(cardsInDiscard[i]);
        cardsInDiscard.Clear();
    }
    public void ReturnDeleted()
    {
        foreach(ICard deletedCard in deletedCards)
            cardsInDeck.Add(deletedCard);
        deletedCards.Clear();
    }
    public void AddToDiscard(ICard card) => cardsInDiscard.Add(card);
    public void AddToTrash(ICard card) => deletedCards.Add(card);
    public void RemoveFromDeck(ICard card) => cardsInDeck.Remove(card);
    public void AddToDeck(ICard card) => cardsInDeck.Add(card);

    public List<ICard> Draw(int amount)
    {
        int availableCards = cardsInDeck.Count + cardsInDiscard.Count;
        amount = amount > availableCards ? availableCards : amount;

        List<ICard> drawnCards = new List<ICard>();
        for(int i = 0; i < amount; i++)
        {
            if(cardsInDeck.Count == 0)
                ReturnDiscard();

            ICard drawn = cardsInDeck.ChooseRandom();
            drawnCards.Add(drawn);
            RemoveFromDeck(drawn);
        }
        return drawnCards;
    }
}