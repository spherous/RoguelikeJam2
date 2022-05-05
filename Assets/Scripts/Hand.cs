using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class Hand : SerializedMonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    public float arc;
    public GameObject cardPosition;
    [OdinSerialize] public List<ICard> cards = new List<ICard>();
    public CardDisplay cardPrefab;
    public float maxAngle;
    public List<CardDisplay> cardList = new List<CardDisplay>();
    private float angle;

    public int handCount;
    public int holdCount;

    private void Awake() {
        waveManager.onWaveStart += OnWaveStart;
        waveManager.onWaveComplete += OnWaveComplete;        
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
            Destroy(cardList[i].gameObject);
            cardList.RemoveAt(i);
        }
    }

    private void OnWaveComplete(Wave wave)
    {
        int numberToDraw = handCount - cardList.Count;
        SpawnCard(numberToDraw);
    }

    public void SpawnCard(int amount = 1)
    {
        for(int i = 0; i < amount; i++)
        {
            CardDisplay newCard = Instantiate(cardPrefab, cardPosition.transform.position, Quaternion.identity, cardPosition.transform);
            newCard.SetCard(cards.ChooseRandom());
            cardList.Add(newCard);
        }

        FanCards();
    }

    public void RemoveCard(CardDisplay card)
    {
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
