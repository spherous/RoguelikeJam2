using System;
using UnityEngine;

public enum BuffType {None = 0, IncreaseAllTowerRange = 1, IncreaseAllTowerDamage = 2, Increase1TowerRange = 3, Increase1TowerDamage = 4, IncreaseAllWebEffects = 5, IncreaseAllWebEffectAttachedToTower = 7, IncreasesChainCount = 8, ReduceChainCount = 9, IncreaseChainRange = 10, ReduceChainRange = 11, IncreaseAllTowerAttackSpeed = 12, Increase1TowerAttackSpeed = 13}

public static class BuffTypeExtensions
{
    public static Action GetEffect(this BuffType type) => type switch{
        BuffType.IncreaseAllTowerRange => () => AdjustGlobalTowerRange(0.1f),
        BuffType.IncreaseAllTowerDamage => () => AdjustGlobalTowerDamage(0.1f),
        BuffType.IncreaseAllWebEffects => () => {},
        BuffType.IncreasesChainCount => () => {},
        BuffType.ReduceChainCount => () => {},
        BuffType.IncreaseChainRange => () => {},
        BuffType.ReduceChainRange => () => {},
        BuffType.IncreaseAllTowerAttackSpeed => () => AdjustGlobalTowerAttackSpeed(0.1f),
        _ => null
    };

    public static Action PlayOnTower(this BuffType type, ITower tower) => type switch{
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
}