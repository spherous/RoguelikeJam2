using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Tech Card", menuName = "Tech Card")]

public class TechCard : ScriptableObject, ICard
{
    public List<TechCardType> techCardTypes = new List<TechCardType>();
    [field:SerializeField] public string description {get; set;}
    [field:SerializeField] public Sprite artwork {get; set;}
    [field:SerializeField] public int threadCost {get; set;}



    public bool TryPlay(Tile tile)
    {
        ThreadPool threadPool = GameObject.FindObjectOfType<ThreadPool>();

        if(threadPool != null && threadPool.Request(threadCost))
        {
            foreach(TechCardType techCardType in techCardTypes)
            {
                techCardType.GetEffect()?.Invoke();
            }
            return true;
        }
        
        return false;
    }

}
