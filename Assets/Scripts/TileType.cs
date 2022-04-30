using UnityEngine;

public enum TileType {Path = 0, Home = 1, EnemySpawnPoint = 2, Buildable = 3}

public static class TileTypeExtensios
{
    public static Color GetColor(this TileType type) => type switch {
        TileType.Home => Color.green,
        TileType.EnemySpawnPoint => Color.red,
        TileType.Buildable => Color.blue,
        _ => Color.white
    };
}