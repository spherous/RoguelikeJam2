using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TechCardType
{

    Draw2 = 0,


}

public static class TechCardTypeExtensions
{
    public static Action GetEffect(this TechCardType techCardType) => techCardType switch
    {
        TechCardType.Draw2 => () => GameObject.FindObjectOfType<CardSpawner>()?.SpawnCard(2),
        _ => null
    };

}

