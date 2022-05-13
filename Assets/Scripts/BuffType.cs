using System;
using UnityEngine;

public enum BuffType {
    None = 0, 
    IncreaseAllTowerRange = 1, 
    IncreaseAllTowerDamage = 2, 
    Increase1TowerRange = 3, 
    Increase1TowerDamage = 4, 
    IncreaseAllWebEffects = 5, 
    IncreaseAllWebEffectAttachedToTower = 7, 
    IncreaseAllChainCount = 8, 
    ReduceAllChainCount = 9, 
    IncreaseAllChainRange = 10, 
    ReduceAllChainRange = 11, 
    IncreaseAllTowerAttackSpeed = 12, 
    Increase1TowerAttackSpeed = 13,
    IncreaseAllChainDamage = 14,
    ReduceAllChainDamage = 15,
    Increase1ChainCount = 16,
    Increase1ChainRange = 17,}

public static class BuffTypeExtensions
{
    public static Action GetEffect(this BuffType type) => type switch{
        BuffType.IncreaseAllTowerRange => () => AdjustGlobalTowerRange(0.1f),
        BuffType.IncreaseAllTowerDamage => () => AdjustGlobalTowerDamage(0.1f),
        BuffType.IncreaseAllWebEffects => () => {},
        BuffType.IncreaseAllChainCount => () => AdjustGlobalChainCount(1),
        BuffType.ReduceAllChainCount => () => AdjustGlobalChainCount(-1),
        BuffType.IncreaseAllChainRange => () => AdjustGlobalChainRange(0.1f),
        BuffType.ReduceAllChainRange => () => AdjustGlobalChainRange(-0.1f),
        BuffType.IncreaseAllChainDamage => () => AdjustGlobalChainDamage(0.1f),
        BuffType.ReduceAllChainDamage => () => AdjustGlobalChainDamage(-0.1f),
        BuffType.IncreaseAllTowerAttackSpeed => () => AdjustGlobalTowerAttackSpeed(0.1f),
        _ => null
    };

    public static Action PlayOnTower(this BuffType type, ITower tower) => type switch{
        BuffType.Increase1ChainCount => () => Adjust1ChainCount(tower,1),
        BuffType.Increase1ChainRange => () => Adjust1ChainRange(tower,0.1f),
        BuffType.Increase1TowerRange => () => tower.AdjustRange(0.66f),
        BuffType.Increase1TowerDamage => () => tower.AdjustDamage(0.66f),
        BuffType.Increase1TowerAttackSpeed => () => tower.AdjustAttackSpeed(0.66f),
        BuffType.IncreaseAllWebEffectAttachedToTower => () => {},
        _ => null
    };

    public static void AdjustGlobalTowerRange(float percentChange)
    {
        TowerManager towerManager = GameObject.FindObjectOfType<TowerManager>();
        towerManager.AdjustTowerRange(percentChange);
    }

    public static void AdjustGlobalTowerDamage(float percentChange)
    {
        TowerManager towerManager = GameObject.FindObjectOfType<TowerManager>();
        towerManager.AdjustTowerDamage(percentChange);
    }
    public static void AdjustGlobalTowerAttackSpeed(float percentChange)
    {
        TowerManager towerManager = GameObject.FindObjectOfType<TowerManager>();
        towerManager.AdjustTowerAttackSpeed(percentChange);
    }
    public static void AdjustGlobalChainCount(int increase)
    {
        ITower[] towers = GameObject.FindObjectsOfType<ChainTower>();
        foreach (ITower tower in towers)
            if (tower is ChainTower chainTower)
                chainTower.AdjustChainCount(increase);
    }
    public static void AdjustGlobalChainRange(float percentChange)
    {
        ITower[] towers = GameObject.FindObjectsOfType<ChainTower>();
        foreach (ITower tower in towers)
            if (tower is ChainTower chainTower)
                chainTower.AdjustChainRange(percentChange);
    }
    public static void AdjustGlobalChainDamage(float percentChange)
    {
        ITower[] towers = GameObject.FindObjectsOfType<ChainTower>();
        foreach (ITower tower in towers)
            if (tower is ChainTower chainTower)
                chainTower.AdjustDamage(percentChange);
    }
    public static void Adjust1ChainCount(ITower tower, int increase)
    {
        if (tower is ChainTower chainTower)
            chainTower.AdjustChainCount(increase);
    }
    public static void Adjust1ChainRange(ITower tower, float percentChange)
    {
        if (tower is ChainTower chainTower)
            chainTower.AdjustChainRange(percentChange);
    }

}