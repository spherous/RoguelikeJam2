using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProcGen : MonoBehaviour
{
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private AstarPath pathfinder;
    public Tile homeTile {get; private set;}
    public List<Tile> spawnPoints {get; private set;} = new List<Tile>();

    [Button]
    public void Generate()
    {
        gridGenerator.GenerateGrid(11, 10);

        PlaceHome();
        PlaceSpawnPoints(1);
        PlaceEdgeWalls();
        GenerateNavMesh();
        ExtendPath();
    }

    private void PlaceHome()
    {
        homeTile = gridGenerator.GetRandomRightEdgeTile();
        homeTile.SetType(TileType.Home);
    }

    private void PlaceSpawnPoints(int count)
    {
        ClearSpawnPoints();
        
        for(int i = 0; i < count; i++)
        {
            if(i >= gridGenerator.height - 2) // No more tiles available on edge, corners don't count, so -2
                break;

            Tile maybeNewSpawnPoint;
            do maybeNewSpawnPoint = gridGenerator.GetRandomLeftEdgeTile();
            while(maybeNewSpawnPoint.type != TileType.Path);

            maybeNewSpawnPoint.SetType(TileType.EnemySpawnPoint);
            spawnPoints.Add(maybeNewSpawnPoint);
        }
    }

    void ClearSpawnPoints()
    {
        foreach(Tile spawnPoint in spawnPoints)
            spawnPoint.SetType(TileType.Path);
        spawnPoints.Clear();
    }

    private void PlaceEdgeWalls()
    {
        var edgeTiles = gridGenerator.GetAllEdgeTiles();
        foreach(Tile tile in edgeTiles)
        {
            if(tile.type != TileType.Path)
                continue;
            tile.SetType(TileType.Buildable);
        }
    }

    private void ExtendPath()
    {
        // Path path = gridGenerator.GetPath(homeTile, spawnPoints[0]);
    }

    private void DebugPath(Path p)
    {
        Debug.Log($"Path length: {p.path.Count}");
        
        foreach(GraphNode node in p.path)
            Debug.Log((Vector3)node.position);
    }

    private void GenerateNavMesh()
    {
        GridGraph graph = pathfinder.data.AddGraph(typeof(GridGraph)) as GridGraph;
        graph.is2D = true;
        graph.collision.use2D = true;
        graph.SetDimensions(gridGenerator.height, gridGenerator.width, 1);
        graph.collision.diameter = 0.1f;

        graph.center = new Vector3(
            x: gridGenerator.width / 2 - (gridGenerator.height.IsEven() ? 0.5f : 0),
            y: gridGenerator.height / 2 - (gridGenerator.width.IsEven() ? 0.5f : 0),
            z: 0
        );

        graph.collision.mask = LayerMask.GetMask("Nonwalkable");

        graph.Scan();
    }
}