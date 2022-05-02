using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class TowerCard : ScriptableObject, ICard
{
    public new string name;
    public string description;
    public Sprite artwork;
    
    public int threadCost;



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
