using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class Deck : SerializedMonoBehaviour
{
    public int startingDeckSize = 5;
    [SerializeField] private LevelManager levelManager;
    public List<ICard> cardsInDeck = new List<ICard>();
    public List<ICard> cardsInDiscard = new List<ICard>();
    public List<ICard> deletedCards = new List<ICard>();
    public bool useDebugDeck = false;
    [OdinSerialize] public List<ICard> startingCards = new List<ICard>();
    void Awake()
    {
        if(useDebugDeck)
        {
            for(int i = 0; i < startingCards.Count; i++)
                cardsInDeck.Add(startingCards[i]);
        }
        else
            GenerateInitialDeck();
        
        levelManager.onLevelComplete += OnLevelComplete;
    }

    private void OnDestroy() => levelManager.onLevelComplete -= OnLevelComplete;
    private void OnLevelComplete(Level level) => ReturnDeleted();

    private void GenerateInitialDeck()
    {
        ICard[] allCards = AssetLoader.GetAllCards();
        TowerType initialTowerType = (TowerType)UnityEngine.Random.Range(0, 4);
        int startingTowerCards = initialTowerType == TowerType.Web ? UnityEngine.Random.Range(2, 4) : UnityEngine.Random.Range(1, 4);
        ICard startingTowrCard = allCards.Where(card => card.type == CardType.Tower && ((TowerCard)card).towerToSpawn == initialTowerType).First();
        List<ICard> allWebMods = allCards.Where(card => card.type == CardType.Environment 
            && ((EnvironmentCard)card).enviroTypes.Any(type => type == EnvironmentType.DamageAmpMod)
            && ((EnvironmentCard)card).enviroTypes.Any(type => type == EnvironmentType.SlowMod)
            && ((EnvironmentCard)card).enviroTypes.Any(type => type == EnvironmentType.StunMod)
            && ((EnvironmentCard)card).enviroTypes.Any(type => type == EnvironmentType.ThornsMod)
            && ((EnvironmentCard)card).enviroTypes.Any(type => type == EnvironmentType.WoundingMod)
        ).ToList();
        
        for(int i = 0; i < startingTowerCards; i++)
            cardsInDeck.Add(startingTowrCard);

        if(initialTowerType == TowerType.Web && allWebMods.Any())
        {
            int startingWebMods = startingDeckSize - startingTowerCards;
            for(int i = 0; i < startingWebMods; i++)
                cardsInDeck.Add(allWebMods.ChooseRandom());
            
            return;
        }

        List<ICard> allNonTowerNonWebModCards = allCards.Where(card => card.type != CardType.Tower).Except(allWebMods).ToList();
        for(int i = 0; i < startingDeckSize - startingTowerCards; i++)
            cardsInDeck.Add(allNonTowerNonWebModCards.ChooseRandom());
    }

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