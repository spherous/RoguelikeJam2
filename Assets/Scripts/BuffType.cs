using System;

public enum BuffType {None = 0, IncreaseAllTowerRange = 1, IncreaseAllTowerDamage = 2, Increase1TowerRange = 3, Increase1TowerDamage = 4, IncreaseAllWebEffects = 5, Increase1WebEffect = 6, IncreaseAllWebEffectAttachedToTower = 7, IncreasesChainCount = 8, ReduceChainCount = 9, IncreaseChainRange = 10, ReduceChainRange = 11, IncreaseAllTowerAttackSpeed = 12, Increase1TowerAttackSpeed = 13}

public static class BuffTypeExtensions
{
    public static Action GetEffect(this BuffType type) => type switch{
        BuffType.IncreaseAllTowerRange => () => {},
        BuffType.IncreaseAllTowerDamage => () => {},
        BuffType.Increase1TowerRange => () => {},
        BuffType.Increase1TowerDamage => () => {},
        BuffType.IncreaseAllWebEffects => () => {},
        BuffType.Increase1WebEffect => () => {},
        BuffType.IncreaseAllWebEffectAttachedToTower => () => {},
        BuffType.IncreasesChainCount => () => {},
        BuffType.ReduceChainCount => () => {},
        BuffType.IncreaseChainRange => () => {},
        BuffType.ReduceChainRange => () => {},
        BuffType.IncreaseAllTowerAttackSpeed => () => {},
        BuffType.Increase1TowerAttackSpeed => () => {},
        _ => null
    };
}