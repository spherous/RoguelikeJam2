using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TechCardType
{

    Draw2 = 0,
    Keep2 = 1,
    Draw1Less = 2,
    Draw1More = 3,




}

public static class TechCardTypeExtensions
{
    public static Action GetEffect(this TechCardType techCardType) => techCardType switch
    {
        TechCardType.Draw2 => () => GameObject.FindObjectOfType<Hand>()?.SpawnCard(2),
        TechCardType.Keep2 => Keep2,
        TechCardType.Draw1Less => () => ModifyDrawCount(-1),
        TechCardType.Draw1More => () => ModifyDrawCount(1),
        _ => null
    };
    public static void Keep2()
    {
        Hand hand = GameObject.FindObjectOfType<Hand>();
        if (hand != null)
            hand.holdCount += 2;

    }
    public static void ModifyDrawCount(int amount)
    {
        Hand hand = GameObject.FindObjectOfType<Hand>();
        if (hand != null)
            hand.handCount += amount;
    }

}

