using System;
using UnityEngine;
public enum TechCardType
{
    Draw2 = 0,
    Keep2 = 1,
    Draw1Less = 2,
    Draw1More = 3,
    Draw3 = 4,
    DiscardHand = 5,
    Heal = 6,

}

public static class TechCardTypeExtensions
{
    public static Action GetEffect(this TechCardType techCardType) => techCardType switch
    {
        TechCardType.Draw2 => () => GameObject.FindObjectOfType<Hand>()?.SpawnCountCheck(2),
        TechCardType.Draw3 => () => GameObject.FindObjectOfType<Hand>()?.SpawnCountCheck(3),
        TechCardType.Keep2 => Keep2,
        TechCardType.Draw1Less => () => ModifyDrawCount(-1),
        TechCardType.Draw1More => () => ModifyDrawCount(1),
        TechCardType.DiscardHand => () => GameObject.FindObjectOfType<Hand>()?.DiscardHand(),
        TechCardType.Heal => () => Heal(1),
        _ => null
    };

    public static void Keep2()
    {
        Hand hand = GameObject.FindObjectOfType<Hand>();
        if(hand != null)
            hand.holdCount += 2;
    }

    public static void ModifyDrawCount(int amount)
    {
        Hand hand = GameObject.FindObjectOfType<Hand>();
        if(hand != null)
            hand.handCount += amount;
    }

    public static void Heal(float amount)
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        if(gm != null)
            gm.Heal(amount);
    }
}