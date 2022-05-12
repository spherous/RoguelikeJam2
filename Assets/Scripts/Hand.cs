using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Deck deck;
    [SerializeField] private BuildMode buildMode;
    [SerializeField] private CardSelection cardSelection;
    public GameObject cardPosition;
    public List<CardDisplay> cardList = new List<CardDisplay>();
    public CardDisplay cardPrefab;
    
    public float arc;
    public float maxAngle;
    private float angle;

    public int handCount;
    public int holdCount;
    private int defaultHandCount;
    private int defaultHoldCount;

    private void Awake()
    {
        defaultHandCount = handCount;
        defaultHoldCount = holdCount;
        waveManager.onWaveStart += OnWaveStart;
        waveManager.onWaveComplete += OnWaveComplete; 
        levelManager.onLevelStart += OnLevelStart;   
    }

    private void OnLevelStart(Level level)
    {
        handCount = defaultHandCount;
        holdCount = defaultHoldCount;
        SpawnCard(handCount - cardList.Count);
    }

    public void DiscardHand()
    {
        for(int i = 0 ; i < cardList.Count; i++)
        {
            deck.AddToDiscard(cardList[i].card);
            Destroy(cardList[i].gameObject);
        }
        cardList.Clear();
    }

    private void OnDestroy()
    {
        waveManager.onWaveStart -= OnWaveStart;
        waveManager.onWaveComplete -= OnWaveComplete;
        levelManager.onLevelStart -= OnLevelStart;           
    }

    private void OnWaveStart(Wave newWave)
    {
        if (buildMode.buildModeOn)
        {
            buildMode.Cancelled();
            buildMode.MoveATowerCancelled();
        }


        if (holdCount <= 0)
            OnWaveStartDiscard(cardList);
        else
            cardSelection.StartSelecting();
        
    }
    public void OnWaveStartDiscard(List<CardDisplay> cardsToDiscard)
    {
        for (int i = cardsToDiscard.Count-1; i >= 0; i--)
        {
            CardDisplay toDiscard = cardsToDiscard[i];
            RemoveCard(toDiscard);
            Destroy(toDiscard.gameObject);
        }

    }

    private void OnWaveComplete(Wave wave) => SpawnCard(handCount - cardList.Count);

    public void SpawnCard(int amount = 1)
    {
        List<ICard> drawnCards = deck.Draw(amount);
        for(int i = 0; i < drawnCards.Count; i++)
        {
            CardDisplay newCard = Instantiate(cardPrefab, cardPosition.transform.position, Quaternion.identity, cardPosition.transform);
            newCard.SetCard(drawnCards[i]);
            cardList.Add(newCard);
        }
        FanCards();
    }

    public void TrashCard(CardDisplay card)
    {
        deck.AddToTrash(card.card);
        cardList.Remove(card);
        FanCards();
    }

    public void RemoveCard(CardDisplay card)
    {
        deck.AddToDiscard(card.card);
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