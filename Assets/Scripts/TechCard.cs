using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tech Card", menuName = "Tech Card")]
public class TechCard : ScriptableObject, ICard
{
    public List<TechCardType> techCardTypes = new List<TechCardType>();
    [field:SerializeField] public string description {get; set;}
    [field:SerializeField] public CardType type {get; set;} = CardType.Tech;
    [field:SerializeField] public Sprite artwork {get; set;}
    [field:SerializeField] public int threadCost {get; set;}
    [field:SerializeField] public bool singleUse {get; set;}
    [field:SerializeField] public int threadUseDuration {get; set;}

    public bool TryPlay(Tile tile)
    {
        ThreadPool threadPool = GameObject.FindObjectOfType<ThreadPool>();
        if(threadPool == null)
            return false;
        else if(threadUseDuration > 0)
            return TryReserveThreads(threadPool);
        return TrySpendThreads(threadPool);
    }

    private bool TrySpendThreads(ThreadPool threadPool)
    {
        if(threadPool.Request(threadCost))
        {
            PlayAllEffects();
            return true;
        }

        return false;
    }

    private void PlayAllEffects()
    {
        foreach(TechCardType techCardType in techCardTypes)
            techCardType.GetEffect()?.Invoke();
    }

    private bool TryReserveThreads(ThreadPool threadPool) => 
        threadPool.RequestReserve(threadCost, threadUseDuration, PlayAllEffects);
}