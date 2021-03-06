using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProcGen : MonoBehaviour
{
    [SerializeField] private Kingdom kingdomPrefab;
    [SerializeField] private GameObject usbPrefab;
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private AstarPath pathfinder;
    public Tile homeTile {get; private set;}
    public Tile spawnPoint {get; private set;}
    public int passes = 3;
    // public int rands = 5;
    public Kingdom kingdom;
    public GameObject usb;

    [ShowInInspector, ReadOnly] public List<Tile> path {get; private set;} = new List<Tile>();

    private void Awake() {
        // Generate(13, 22);
    }

    [Button]
    public void Generate(int row, int col, int chaos = 1)
    {
        gridGenerator.GenerateGrid(row, col);

        PlaceHome();
        PlaceSpawnPoints(1);
        PlaceEdgeWalls();
        GenerateNavMesh();
        ExtendPath(chaos);
        PlaceSprites();
        CachePath();
    }

    private void PlaceSprites()
    {
        foreach(var col in gridGenerator.grid)
        {
            foreach(Tile tile in col)
            {
                if(tile == null)
                    continue;
                else if(tile.type == TileType.Buildable || tile.type == TileType.Path)
                    tile.UpdateSprite();
            }
        }
    }

    public void CachePath() => path = SelectPath(gridGenerator.GetPath(spawnPoint.index, homeTile.index));

    private List<Tile> SelectPath(Path p) => p.path.Select(node =>
    {
        Vector3 nodePos = (Vector3)node.position;
        Index nodeIndex = new Index(nodePos.y.Floor(), nodePos.x.Floor());
        return gridGenerator.TryGetTile(nodeIndex, out Tile tile) ? tile : null;
    }).ToList();

    private void PlaceHome()
    {
        homeTile = gridGenerator.GetRandomRightEdgeTile();
        homeTile.SetType(TileType.Home);
        if(kingdom == null)
            kingdom = Instantiate(kingdomPrefab, homeTile.transform.position + Vector3.right * 5.2f, Quaternion.identity);
        else
            kingdom.transform.position = homeTile.transform.position + Vector3.right * 5.2f;
    }

    private void PlaceSpawnPoints(int count)
    {
        ClearSpawnPoints();
        
        Tile maybeNewSpawnPoint;
        do maybeNewSpawnPoint = gridGenerator.GetRandomLeftEdgeTile();
        while(maybeNewSpawnPoint.type != TileType.Path);

        maybeNewSpawnPoint.SetType(TileType.EnemySpawnPoint);
        spawnPoint = maybeNewSpawnPoint;
        if(usb == null)
            usb = Instantiate(usbPrefab, spawnPoint.transform.position + Vector3.left * 5f, Quaternion.identity);
        else
            usb.transform.position = spawnPoint.transform.position + Vector3.left * 5f;
    }

    void ClearSpawnPoints() => spawnPoint?.SetType(TileType.Path);

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

    private void ExtendPath(int chaos = 1)
    {
        Path currentPath = gridGenerator.GetPath(spawnPoint, homeTile);

        if(currentPath == null)
            return;

        Vector3 firstPathTilePos = (Vector3)currentPath.path[1].position;
        Vector3 lastPathTilePos = (Vector3)currentPath.path[currentPath.path.Count - 1].position;
        Index firstPathTileIndex = new Index(firstPathTilePos.y.Floor(), firstPathTilePos.x.Floor());
        Index lastPathTileIndex = new Index(lastPathTilePos.y.Floor(), lastPathTilePos.x.Floor());

        // These two tiles should never be changed, it will always invalidate the path.
        Tile firstTile = gridGenerator.TryGetTile(firstPathTileIndex, out Tile firstTileResult) ? firstTileResult : null;
        Tile lastTile = gridGenerator.TryGetTile(lastPathTileIndex, out Tile lastTileResult) ? lastTileResult : null;

        if(firstTile == null || lastTile == null) // If these tiles are null, the path is invalid
            return;

        int prevPathLength = currentPath.path.Count;
        
        List<Tile> currentTilePath = SelectPath(currentPath);
        List<Tile> allPathTiles = gridGenerator.GetAllPathTiles();

        List<Tile> allPathTilesWithoutCurrentPath = allPathTiles.Except(currentTilePath).ToList();
        List<Tile> randomizedTiles = new List<Tile>();
        for(int rand = 0; rand < chaos; rand++)
        {
            Tile toRand;
            do toRand = allPathTilesWithoutCurrentPath.ChooseRandom();
            while(randomizedTiles.Contains(toRand));

            // We are not changing a tile that could obstruct the valid path, so we don't care about the return value
            UpdateTileAlongPath(toRand, TileType.Buildable);
        }

        for(int pass = 0; pass < passes; pass++)
        {
            for(int i = 2; i < currentPath.path.Count - 2; i++) // skip 1st 2 and last 2, changing those would invalidate the path
            {
                Vector3 nodePos = (Vector3)currentPath.path[i].position;
                Index nodeIndex = new Index(nodePos.y.Floor(), nodePos.x.Floor());

                if(gridGenerator.TryGetTile(nodeIndex, out Tile tile))
                {
                    // try to change tile to buildable, if that makes path shorter: change back.
                    // if path is left at same length or longer, leave as buildable, move to next tile and try again.
                    TileType orgType = tile.type;

                    currentPath = UpdateTileAlongPath(tile, TileType.Buildable);

                    // if the final tile in the new path is not the home tile, the path is not valid
                    Vector3 finalTilePos = (Vector3)currentPath.path[currentPath.path.Count - 1].position;
                    Index finalTileIndex = new Index(finalTilePos.y.Floor(), finalTilePos.x.Floor());

                    if(currentPath.path.Count < prevPathLength || finalTileIndex != homeTile.index)
                    {
                        currentPath = UpdateTileAlongPath(tile, orgType);
                        continue;
                    }
                    else if(currentPath.path.Count == prevPathLength)
                    {
                        // Placing a buildable path here doesn't change the length of the path, so let's randomly choose if we revert or not for more interesting generation.
                        if(UnityEngine.Random.Range(0, 2).IsEven())
                        {
                            currentPath = UpdateTileAlongPath(tile, orgType);
                            continue;
                        }                        
                    }

                    // If we don't shuffle, the path favors going down.
                    List<Tile> neighbors = gridGenerator.GetAdjacentTiles(tile).Shuffle();
                    // Try changing neighboring path tiles to buildable, if that makes path shorter or invalid: change back.
                    foreach(Tile neighbor in neighbors)
                    {
                        // don't invalidate the path
                        if(neighbor == null || neighbor.type != TileType.Path || neighbor == firstTile || neighbor == lastTile)
                            continue;

                        currentPath = UpdateTileAlongPath(neighbor, TileType.Buildable);

                        finalTilePos = (Vector3)currentPath.path[currentPath.path.Count - 1].position;
                        finalTileIndex = new Index(finalTilePos.y.Floor(), finalTilePos.x.Floor());

                        if(currentPath.path.Count < prevPathLength || finalTileIndex != homeTile.index)
                        {
                            currentPath = UpdateTileAlongPath(neighbor, orgType);
                            continue;
                        }
                        else if(currentPath.path.Count == prevPathLength)
                        {
                            // Placing a buildable path here doesn't change the length of the path, so let's randomly choose if we revert or not for more interesting generation.
                            if(UnityEngine.Random.Range(0, 2).IsEven())
                            {
                                currentPath = UpdateTileAlongPath(tile, orgType);
                                continue;
                            }                        
                        }
                    }

                    prevPathLength = currentPath.path.Count;
                }
            }
        }
    }

    public Path UpdateTileAlongPath(Tile tile, TileType type)
    {
        Path path;
        tile.SetType(type);
        pathfinder.Scan(); // rescan to update navmesh
        path = gridGenerator.GetPath(spawnPoint, homeTile);
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
        graph.cutCorners = false;
        graph.neighbours = NumNeighbours.Four;

        graph.center = new Vector3(
            x: gridGenerator.height / 2 - (gridGenerator.height.IsEven() ? 0.5f : 0),
            y: gridGenerator.width / 2 - (gridGenerator.width.IsEven() ? 0.5f : 0),
            z: 0
        );

        graph.collision.mask = LayerMask.GetMask("Nonwalkable");

        graph.Scan();
    }

    public void CreateBuildableTile(Tile tile)
    {
        Path path = UpdateTileAlongPath(tile, TileType.Buildable);
        CachePath();       
        tile.UpdateSprite();
        foreach(Tile t in gridGenerator.GetAdjacentTiles(tile))
        {
            if(t.type == TileType.Buildable || t.type == TileType.Path)
                t.UpdateSprite();
        }
    }

    public void RemoveBuildable(Tile tile)
    {
        Path path = UpdateTileAlongPath(tile, TileType.Path);
        CachePath();
        foreach(Tile t in gridGenerator.GetAdjacentTiles(tile))
        {
            if(t.type == TileType.Buildable)
                t.UpdateSprite();
        }
    }

    public bool ChangeToBuildableIsValid(Tile tile)
    {
        if(tile.type != TileType.Path) // Only paths may become buildable
            return false;

        TileType org = tile.type;
        Path currentPath = gridGenerator.GetPath(spawnPoint, homeTile);
        Path path = UpdateTileAlongPath(tile, TileType.Buildable);
        
        if(path.path.Count < currentPath.path.Count)
        {
            // shrinking the path is never valid when placing a build tile.
            path = UpdateTileAlongPath(tile, org);
            return false;
        }

        // valid location, revert tile
        path = UpdateTileAlongPath(tile, org);
        return true;
    }

    // changing to a path can NEVER obstruct the path, so we don't need to check for obstructions to validate, only the tile type
    public bool ChangeToPathIsValid(Tile tile) => tile.type == TileType.Buildable;
}