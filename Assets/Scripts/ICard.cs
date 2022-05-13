using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
    string name {get; set;}
    string description {get; set;}
    CardType type {get; set;}
    Sprite artwork {get; set;}
    int threadCost {get; set;}
    int threadUseDuration {get; set;}
    ThreadReserveType threadReserveType {get; set;}
    ThreadEffectTriggerCondition threadEffectTriggerCondition {get; set;}
    bool singleUse {get; set;}
    List<AudioClip> audioClips {get; set;}
    bool TryPlay(Tile tile);   
}