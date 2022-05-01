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
    public void SpawnCard()
    {
        
        
        float angle = Mathf.Clamp(arc/(cardCount-1), 0, maxAngle);
        int rand = Random.Range(0, cards.Length);
        Debug.Log(rand);
        GameObject newCard = Instantiate(cardPrefab, cardPosition.transform.position, Quaternion.identity, cardPosition.transform);
        cardCount++;
        newCard.GetComponent<CardDisplay>().card = cards[rand] as TowerCard;
        cardList.Add(newCard);
        bool isEven = cardCount.IsEven();
        angle = isEven? angle/2f : angle;
        for (int i = 0; i < cardCount; i++){
            if (isEven)
            {
                
            }
            else
            {
                float theta = (((((float)cardCount/2f).Floor() * angle)*-1)+i*angle)+90;
                Debug.Log(theta);
                theta = theta * Mathf.Deg2Rad;
                Vector2 potato = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
                cardList[i].transform.localPosition = potato * 900;
                cardList[i].transform.up = potato;
                 
            }
        }

        

    }
}
