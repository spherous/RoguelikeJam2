using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    private int cardCount = 0;
    public GameObject[] cardPositions;
    public ScriptableObject[] cards;
    public GameObject cardPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(cards.Length);
    }
    public void SpawnCard()
    {
        int rand = Random.Range(0, cards.Length);
        Debug.Log(rand);
        GameObject newCard = Instantiate(cardPrefab, cardPositions[cardCount].transform.position, Quaternion.identity, FindObjectOfType<Canvas>().transform);
        cardCount++;
        newCard.GetComponent<CardDisplay>().card = cards[rand] as TowerCard;
    }
}
