using System;

public enum EnvironmentType {
    None = 0,
    CreateBuildableTile = 1,
    RemoveBuildableTile = 2,
    SlowMod = 8,
    StunMod = 9,
    ThornsMod = 10,
    WoundingMod = 11,
    DamageAmpMod = 12,
}

public static class EnvironmentTypeExtensions
{
    public static Action GetEffect(this EnvironmentType type) => type switch{
          
        _ => null
    };

    public static Action PlayOnTower(this EnvironmentType environmentType, ITower tower) => environmentType switch
    {
        EnvironmentType.SlowMod => () => GrantWebEffect(tower, WebEffect.Slow),
        EnvironmentType.StunMod => () => GrantWebEffect(tower, WebEffect.Stun),
        EnvironmentType.ThornsMod => () => GrantWebEffect(tower, WebEffect.Thorns),
        EnvironmentType.WoundingMod => () => GrantWebEffect(tower, WebEffect.Wounding),
        EnvironmentType.DamageAmpMod => () => GrantWebEffect(tower, WebEffect.DamageAmp),
        _ => null
    };

    public static Action PlayOnTile(this EnvironmentType type, Tile tile) => type switch{
        EnvironmentType.CreateBuildableTile => () => CreateBuildable(tile),
        EnvironmentType.RemoveBuildableTile => () => RemoveBuildable(tile),
        _ => null
    };

    private static void RemoveBuildable(Tile tile)
    {
        
    }

    private static void CreateBuildable(Tile tile)
    {
        
    }

    public static void GrantWebEffect(ITower tower, WebEffect webEffect)
    {
        if(tower is WebTower webTower)
            webTower.GainWebEffect(webEffect);
    }
}