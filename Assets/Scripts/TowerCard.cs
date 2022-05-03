using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class TowerCard : ScriptableObject, ICard
{
    [field:SerializeField] public string description {get; set;}
    [field:SerializeField] public Sprite artwork {get; set;}
    [field:SerializeField] public int threadCost {get; set;}

    public bool TryPlay()
    {
        ThreadPool threadPool = GameObject.FindObjectOfType<ThreadPool>();

        if(threadPool != null && threadPool.Request(threadCost))
        {
            return true;
        }
        
        return false;
    }
}
