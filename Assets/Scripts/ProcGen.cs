using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProcGen : MonoBehaviour
{
    [SerializeField] private GridGenerator gridGenerator;
    public Tile homeTile {get; private set;}
    public List<Tile> spawnPoints {get; private set;} = new List<Tile>();

    [Button]
    public void Generate()
    {
        gridGenerator.GenerateGrid(10, 10);

        PlaceHome();
        PlaceSpawnPoints(1);
    }

    private void PlaceHome()
    {
        homeTile = gridGenerator.GetRandomEdgeTile();
        homeTile.SetType(TileType.Home);
    }

    private void PlaceSpawnPoints(int count)
    {
        for(int i = 0; i < count; i++)
        {
            Tile maybeNewSpawnPoint;
            do maybeNewSpawnPoint = gridGenerator.GetRandomEdgeTile();
            while(maybeNewSpawnPoint.type == TileType.Home);

            maybeNewSpawnPoint.SetType(TileType.EnemySpawnPoint);
            spawnPoints.Add(maybeNewSpawnPoint);
        }
    }
}