using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    private int cardCount = 0;
    public float arc;
    public GameObject cardPosition;
    public ScriptableObject[] cards;
    public GameObject cardPrefab;
    public float maxAngle;
    private List<GameObject> cardList = new List<GameObject>();
    private float angle;

    public void SpawnCard()
    {
        angle = Mathf.Clamp(arc/(cardCount-1), 0, maxAngle);
        int rand = Random.Range(0, cards.Length);
        GameObject newCard = Instantiate(cardPrefab, cardPosition.transform.position, Quaternion.identity, cardPosition.transform);
        cardCount++;
        newCard.GetComponent<CardDisplay>().card = cards[rand] as TowerCard;
        cardList.Add(newCard);
        SortCards();
    }
    public void SortCards()
    {
        bool isEven = cardCount.IsEven();
        for (int i = 0; i < cardCount; i++)
        {
                float theta = ((((((float)cardCount/2f).Floor() * angle)*-1)+i*angle)+90);
                if(isEven) theta = theta + (angle/2f);
                theta = theta * Mathf.Deg2Rad;
                Vector2 potato = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
                cardList[i].transform.localPosition = potato * 900;
                cardList[i].transform.up = potato;
        }
    }
}
