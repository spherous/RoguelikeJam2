using System;
using UnityEngine;

public enum BuffType {None = 0, IncreaseAllTowerRange = 1, IncreaseAllTowerDamage = 2, Increase1TowerRange = 3, Increase1TowerDamage = 4, IncreaseAllWebEffects = 5, Increase1WebEffect = 6, IncreaseAllWebEffectAttachedToTower = 7, IncreasesChainCount = 8, ReduceChainCount = 9, IncreaseChainRange = 10, ReduceChainRange = 11, IncreaseAllTowerAttackSpeed = 12, Increase1TowerAttackSpeed = 13}

public static class BuffTypeExtensions
{
    public static Action GetEffect(this BuffType type) => type switch{
        BuffType.IncreaseAllTowerRange => () => AdjustTowerRange(0.5f),
        BuffType.IncreaseAllTowerDamage => () => AdjustTowerDamage(0.5f),
        BuffType.Increase1TowerRange => () => {},
        BuffType.Increase1TowerDamage => () => {},
        BuffType.IncreaseAllWebEffects => () => {},
        BuffType.Increase1WebEffect => () => {},
        BuffType.IncreaseAllWebEffectAttachedToTower => () => {},
        BuffType.IncreasesChainCount => () => {},
        BuffType.ReduceChainCount => () => {},
        BuffType.IncreaseChainRange => () => {},
        BuffType.ReduceChainRange => () => {},
        BuffType.IncreaseAllTowerAttackSpeed => () => AdjustTowerAttackSpeed(0.5f),
        BuffType.Increase1TowerAttackSpeed => () => {},
        _ => null
    };

    public static void AdjustTowerRange(float percentChange)
    {
        TowerManager towerManager = GameObject.FindObjectOfType<TowerManager>();
        towerManager.AdjustTowerRange(percentChange);
    }

    public static void AdjustTowerDamage(float percentChange)
    {
        TowerManager towerManager = GameObject.FindObjectOfType<TowerManager>();
        towerManager.AdjustTowerDamage(percentChange);
    }
    public static void AdjustTowerAttackSpeed(float percentChange)
    {
        TowerManager towerManager = GameObject.FindObjectOfType<TowerManager>();
        towerManager.AdjustTowerAttackSpeed(percentChange);
    }
}