using System;
using System.Collections.Generic;
using System.Linq;
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
    [field:SerializeField] public ThreadReserveType threadReserveType {get; set;} = ThreadReserveType.None;
    [field:SerializeField] public ThreadEffectTriggerCondition threadEffectTriggerCondition {get; set;} = ThreadEffectTriggerCondition.OnComplete;
    [field:SerializeField] public int threadUseDuration {get; set;}
    [field:SerializeField] public bool playOnTower {get; set;} = false;

    public bool TryPlay(Tile tile)
    {
        ThreadPool threadPool = GameObject.FindObjectOfType<ThreadPool>();
        if(threadPool == null)
            return false;
        else if(threadUseDuration > 0 || threadReserveType != ThreadReserveType.None)
            return TryReserveThreads(threadPool);
        return TrySpendThreads(threadPool, tile);
    }

    private void PlayAllEffects()
    {
        foreach(TechCardType techCardType in techCardTypes)
            techCardType.GetEffect()?.Invoke();
    }

    private void PlayOnTower(ITower tower)
    {
        foreach(TechCardType techCardType in techCardTypes)
            techCardType.PlayOnTower(tower)?.Invoke();
    }

    private bool TrySpendThreads(ThreadPool threadPool, Tile tile)
    {
        if(techCardTypes.Any(type => type == TechCardType.RefreshThread))
        {
            int qualifyingThreads = threadPool.GetUnavailableThreads().Where(thread => thread.remainingCooldown == 0).Count();
            if(qualifyingThreads == 0) // if the player doesn't have any threads on cd, they shouldn't be able to play a refresh thread card
                return false;
        }
        
        if(threadPool.Request(threadCost))
        {
            if(playOnTower && tile != null && tile.tower != null)
                PlayOnTower(tile.tower);
            else
                PlayAllEffects();
            return true;
        }

        return false;
    }

    private bool TryReserveThreads(ThreadPool threadPool) => 
        threadPool.RequestReserve(threadCost, threadUseDuration, PlayAllEffects, threadReserveType, threadEffectTriggerCondition);
}