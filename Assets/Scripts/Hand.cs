using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class Hand : SerializedMonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Deck deck;
    public float arc;
    public GameObject cardPosition;
    [OdinSerialize] public List<ICard> cards = new List<ICard>();
    public List<CardDisplay> cardList = new List<CardDisplay>();
    private List<ICard> cardsDrawn = new List<ICard>();
    public CardDisplay cardPrefab;
    public float maxAngle;
    
    private float angle;

    public int handCount;
    public int holdCount;
    public int LeftToSpawn;

    private void Awake() {
        waveManager.onWaveStart += OnWaveStart;
        waveManager.onWaveComplete += OnWaveComplete;        
    }

    public void DiscardHand()
    {
        for (int i = 0 ; i < cardList.Count; i++)
        {
            deck.moveToDiscard(cardList[i]);
            Destroy(cardList[i].gameObject);
        }
        cardList.Clear();
    }

    private void Start() => SpawnCard(handCount);

    private void OnDestroy()
    {
        waveManager.onWaveStart -= OnWaveStart;
        waveManager.onWaveComplete -= OnWaveComplete;            
    }

    private void OnWaveStart(Wave newWave)
    {
        for(int i = cardList.Count-1; i >= holdCount; i--)
        {
            deck.moveToDiscard(cardList[i]);
            Destroy(cardList[i].gameObject);
            cardList.RemoveAt(i);
        }
    }

    private void OnWaveComplete(Wave wave)
    {
        int numberToDraw = handCount - cardList.Count;
        SpawnCountCheck(numberToDraw);
    }

    void SpawnCard(int amount = 1)
    {
        for(int i = 0; i < amount; i++)
        {
            CardDisplay newCard = Instantiate(cardPrefab, cardPosition.transform.position, Quaternion.identity, cardPosition.transform);
            ICard cardToDraw = deck.cardsInDeck.ChooseRandom();
            newCard.SetCard(cardToDraw);
            cardsDrawn.Add(cardToDraw);
            cardList.Add(newCard);
            deck.moveToHand(newCard);
        }

        FanCards();
    }
    public void SpawnCountCheck(int spawnCount)
    {
        if (spawnCount > deck.cardsInDeck.Count + deck.cardsInDiscard.Count)
        {
            spawnCount = deck.cardsInDeck.Count + deck.cardsInDiscard.Count;
            Debug.Log("not enough cards to draw");
        }
        if(deck.cardsInDeck.Count < spawnCount)
        {
            LeftToSpawn = spawnCount - deck.cardsInDeck.Count;
            SpawnCard(deck.cardsInDeck.Count);
            deck.returnDiscard();
            SpawnCard(LeftToSpawn);
            return;
        }
        SpawnCard(spawnCount);
    }
    public void TrashCard(CardDisplay card)
    {
        deck.moveToTrash(card);
        cardList.Remove(card);
        FanCards();
    }

    public void RemoveCard(CardDisplay card)
    {
        deck.moveToDiscard(card);
        cardList.Remove(card);
        FanCards();
    }

    public void FanCards()
    {
        angle = Mathf.Clamp(arc/(cardList.Count - 1), 0, maxAngle);
        bool isEven = cardList.Count.IsEven();
        for(int i = 0; i < cardList.Count; i++)
        {
            float theta = ((((((float)cardList.Count/2f).Floor() * angle)*-1)+i*angle));
            if(isEven) 
                theta = theta + (angle/2f);

            theta = (theta + 90) * Mathf.Deg2Rad;
            Vector2 desiredVec = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            cardList[i].transform.localPosition = desiredVec * 900;
            cardList[i].transform.up = desiredVec;
        }
    }


}
