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


    public int passes = 3;

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
        Path path = gridGenerator.GetPath(spawnPoints[0], homeTile);

        if(path == null)
            return;

        Vector3 firstPathTilePos = (Vector3)path.path[1].position;
        Vector3 lastPathTilePos = (Vector3)path.path[path.path.Count - 1].position;
        Index firstPathTileIndex = new Index(firstPathTilePos.y.Floor(), firstPathTilePos.x.Floor());
        Index lastPathTileIndex = new Index(lastPathTilePos.y.Floor(), lastPathTilePos.x.Floor());

        // These two tiles should never be changed, it will always invalidate the path.
        Tile firstTile = gridGenerator.TryGetTile(firstPathTileIndex, out Tile firstTileResult) ? firstTileResult : null;
        Tile lastTile = gridGenerator.TryGetTile(lastPathTileIndex, out Tile lastTileResult) ? lastTileResult : null;

        if(firstTile == null || lastTile == null) // If these tiles are null, the path is invalid
            return;

        int prevPathLength = path.path.Count;
        
        for(int pass = 0; pass < passes; pass++)
        {
            for(int i = 2; i < path.path.Count - 2; i++) // skip 1st 2 and last 2, changing those would invalidate the path
            {
                Vector3 nodePos = (Vector3)path.path[i].position;
                Index nodeIndex = new Index(nodePos.y.Floor(), nodePos.x.Floor());

                if(gridGenerator.TryGetTile(nodeIndex, out Tile tile))
                {
                    // try to change tile to buildable, if that makes path shorter: change back.
                    // if path is left at same length or longer, leave as buildable, move to next tile and try again.
                    TileType orgType = tile.type;

                    path = UpdateTileAlongPath(tile, TileType.Buildable);

                    // if the final tile in the new path is not the home tile, the path is not valid
                    Vector3 finalTilePos = (Vector3)path.path[path.path.Count - 1].position;
                    Index finalTileIndex = new Index(finalTilePos.y.Floor(), finalTilePos.x.Floor());

                    if(path.path.Count < prevPathLength || finalTileIndex != homeTile.index)
                    {
                        path = UpdateTileAlongPath(tile, orgType);
                        continue;
                    }

                    // Try changing neighboring path tiles to buildable, if that makes path shorter or invalid: change back.
                    foreach(Tile neighbor in gridGenerator.GetNeighbors(tile))
                    {
                        // don't invalidate the path
                        if(neighbor.type != TileType.Path || neighbor == firstTile || neighbor == lastTile)
                            continue;

                        path = UpdateTileAlongPath(neighbor, TileType.Buildable);

                        finalTilePos = (Vector3)path.path[path.path.Count - 1].position;
                        finalTileIndex = new Index(finalTilePos.y.Floor(), finalTilePos.x.Floor());

                        if(path.path.Count < prevPathLength || finalTileIndex != homeTile.index)
                        {
                            path = UpdateTileAlongPath(neighbor, orgType);
                            continue;
                        }
                    }

                    prevPathLength = path.path.Count;
                }
            }
        }
    }

    private Path UpdateTileAlongPath(Tile tile, TileType type)
    {
        Path path;
        tile.SetType(type);
        pathfinder.Scan(); // rescan to update navmesh
        path = gridGenerator.GetPath(spawnPoints[0], homeTile);
        return path;
    }

    private void DebugPath(Path p)
    {
        Debug.Log($"Path length: {p.path.Count}");
        
        foreach(GraphNode node in p.path)
            Debug.Log((Vector3)node.position);
    }

    private void GenerateNavMesh()
    {
        foreach(var g in pathfinder.data.graphs)
            pathfinder.data.RemoveGraph(g);

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