using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Environment Card", menuName = "Environment Card")]
public class EnvironmentCard : ScriptableObject, ICard
{
    public List<EnvironmentType> enviroTypes = new List<EnvironmentType>();
    [field:SerializeField] public string description {get; set;}
    [field:SerializeField] public CardType type {get; set;} = CardType.Environment;
    [field:SerializeField] public Sprite artwork {get; set;}
    [field:SerializeField] public int threadCost {get; set;}
    [field:SerializeField] public bool singleUse {get; set;}
    [field:SerializeField] public ThreadReserveType threadReserveType {get; set;} = ThreadReserveType.None;
    [field:SerializeField] public ThreadEffectTriggerCondition threadEffectTriggerCondition {get; set;} = ThreadEffectTriggerCondition.OnComplete;
    [field:SerializeField] public int threadUseDuration {get; set;}
    [field:SerializeField] public bool playOnTower {get; set;} = false;
    [field:SerializeField] public List<AudioClip> audioClips {get; set;}
    
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
        foreach(EnvironmentType type in enviroTypes)
            type.GetEffect()?.Invoke();
    }

    private void PlayOnTower(ITower tower)
    {
        foreach(EnvironmentType type in enviroTypes)
            type.PlayOnTower(tower)?.Invoke();
    }
    
    private void PlayOnTile(Tile tile)
    {
        foreach(EnvironmentType type in enviroTypes)
            type.PlayOnTile(tile)?.Invoke();
    }

    private bool TrySpendThreads(ThreadPool threadPool, Tile tile)
    {
        if(threadPool.Request(threadCost))
        {
            if(enviroTypes.Any(type => type == EnvironmentType.CreateBuildableTile || type == EnvironmentType.RemoveBuildableTile))
                PlayOnTile(tile);
            else if(playOnTower && tile != null && tile.tower != null)
                PlayOnTower(tile.tower);
            else
                PlayAllEffects();
            return true;
        }

        return false;
    }

    private bool TryReserveThreads(ThreadPool threadPool)
    {
        if(threadPool.RequestReserve(threadCost, threadUseDuration, PlayAllEffects, threadReserveType, threadEffectTriggerCondition))
        {
            if(threadEffectTriggerCondition == ThreadEffectTriggerCondition.EveryTurn)
                PlayAllEffects();
            
            return true;
        }
        
        return false;
    }
}