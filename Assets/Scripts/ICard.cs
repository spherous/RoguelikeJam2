using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
    string name {get; set;}
    string description {get; set;}
    Sprite artwork {get; set;}
    int threadCost {get; set;}
    int threadUseDuration {get; set;}
    bool singleUse {get; set;}
    bool TryPlay(Tile tile);   
}