using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Debuff Card", menuName = "Debuff Card")]
public class DebuffCard : ScriptableObject, ICard
{
    public List<DebuffType> debuffTypes = new List<DebuffType>();
    [field:SerializeField] public string description {get; set;}
    [field:SerializeField] public CardType type {get; set;} = CardType.Debuff;
    [field:SerializeField] public Sprite artwork {get; set;}
    [field:SerializeField] public int threadCost {get; set;}
    [field:SerializeField] public bool singleUse {get; set;}
    [field:SerializeField] public ThreadReserveType threadReserveType {get; set;} = ThreadReserveType.None;
    [field:SerializeField] public ThreadEffectTriggerCondition threadEffectTriggerCondition {get; set;} = ThreadEffectTriggerCondition.OnComplete;
    [field:SerializeField] public int threadUseDuration {get; set;}
    [field:SerializeField] public List<AudioClip> audioClips {get; set;}

    public bool TryPlay(Tile tile)
    {
        ThreadPool threadPool = GameObject.FindObjectOfType<ThreadPool>();
        if(threadPool == null)
            return false;
        else if(threadUseDuration > 0 || threadReserveType != ThreadReserveType.None)
            return TryReserveThreads(threadPool);
        return TrySpendThreads(threadPool);
    }

    private void PlayAllEffects()
    {
        foreach(DebuffType type in debuffTypes)
            type.GetEffect()?.Invoke();
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