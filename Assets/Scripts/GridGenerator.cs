using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [ShowInInspector, ReadOnly] public List<List<Tile>> grid = new List<List<Tile>>();
    [SerializeField] private Seeker seeker;

    public int width => grid.Count;
    public int height => grid.Count > 0 ? grid[0].Count : 0;

    [Button]
    public void Clear()
    {
        foreach(var row in grid)
        {
            foreach(var tile in row)
                Destroy(tile.gameObject);
        }
        grid.Clear();
    }

    [Button]
    public void GenerateGrid(int rows, int cols)
    {
        Clear();

        for(int row = 0; row < rows; row++)
        {
            grid.Add(new List<Tile>());
            for(int col = 0; col < cols; col++)
            {
                Tile tile = Instantiate(tilePrefab, new Vector3(col, row, 0), Quaternion.identity, transform);
                tile.SetIndex(new Index(row, col));
                grid[row].Add(tile);
            }
        }
    }

    public List<Tile> TryGetTilesWithOffset(Index location, List<Index> rangeOffsets)
    {
        List<Tile> tiles = new List<Tile>();
        foreach(Index offset in rangeOffsets)
        {
            TryGetTile(location + offset, out Tile tile); // We want to perserve order from the incoming offsets, so let's add the null to the list
            tiles.Add(tile);
        }
        return tiles;
    }

    public Path GetPath(Index start, Index end) => 
        TryGetTile(start, out Tile startTile) && TryGetTile(end, out Tile endTile) ? GetPath(startTile, endTile) : null;

    public Path GetPath(Tile start, Tile end)
    {
        if(start == null || end == null)
            return null;
        
        Path path = seeker.StartPath(start.transform.position, end.transform.position);
        AstarPath.BlockUntilCalculated(path);
        return path;
    }

    public Index GetRandomIndex() => GetRandomTile().index;
    public Tile GetRandomTile() => grid.ChooseRandom().ChooseRandom();

    public List<Tile> GetAllEdgeTiles()
    {
        // Doesn't skip corners

        List<Tile> edgeTiles = new List<Tile>();
        foreach(List<Tile> row in grid)
        {
            foreach(Tile tile in row)
            {
                if(tile.index.row == 0 || tile.index.row == grid.Count - 1 || tile.index.col == 0 || tile.index.col == grid[0].Count - 1)
                    edgeTiles.Add(tile);
            }
        }
        return edgeTiles;
    }
    
    public Tile GetRandomEdgeTile()
    {
        List<Tile> edgeTiles = GetAllEdgeTiles();
        return edgeTiles.ChooseRandom();
    }

    public Tile GetRandomLeftEdgeTile()
    {
        List<Tile> edgeTiles = new List<Tile>();
        for (int i = 0; i < grid.Count; i++)
        {
            List<Tile> row = grid[i];
            // Skip corners
            if(i == 0 || i == grid.Count - 1)
                continue;

            foreach (Tile tile in row)
            {
                if(tile.index.col == 0)
                    edgeTiles.Add(tile);
            }
        }
        return edgeTiles.ChooseRandom();
    }

    public Tile GetRandomRightEdgeTile()
    {
        List<Tile> edgeTiles = new List<Tile>();
        for (int i = 0; i < grid.Count; i++)
        {
            List<Tile> row = grid[i];
            // Skip corners
            if(i == 0 || i == grid.Count - 1)
                continue;
                
            foreach(Tile tile in row)
            {
                if(tile.index.col == grid[0].Count - 1)
                    edgeTiles.Add(tile);
            }
        }
        return edgeTiles.ChooseRandom();
    }

    public Tile GetRandomTopEdgeTile()
    {
        List<Tile> edgeTiles = new List<Tile>();
        foreach(List<Tile> row in grid)
        {
            foreach(Tile tile in row)
            {
                // skip corners
                if(tile.index.row == 0 && (tile.index.col != 0 && tile.index.col != grid[0].Count - 1))
                    edgeTiles.Add(tile);
            }
        }
        return edgeTiles.ChooseRandom();
    }

    public Tile GetRandomBottomEdgeTile()
    {
        List<Tile> edgeTiles = new List<Tile>();
        foreach(List<Tile> row in grid)
        {
            foreach(Tile tile in row)
            {
                // skip corners
                if(tile.index.row == grid.Count - 1  && (tile.index.col != 0 && tile.index.col != grid[0].Count - 1)) 
                    edgeTiles.Add(tile);
            }
        }
        return edgeTiles.ChooseRandom();
    }

    public bool TryGetTile(Index index, out Tile tile)
    {
        tile = null;
        if(index.row < 0 || index.row >= grid.Count)
            return false;

        var row = grid[index.row];
        if(index.col < 0 || index.col >= row.Count)
            return false;

        tile = row[index.col];
        return true;
    }

    public List<Tile> GetAdjacentTiles(Tile source)
    {
        IEnumerable<Tile> Adjacent()
        {
            if(TryGetTile(source.index + new Index(0, 1), out Tile upTile))
                yield return upTile;
            if(TryGetTile(source.index + new Index(0, -1), out Tile downTile))
                yield return downTile;
            if(TryGetTile(source.index + new Index(-1, 0), out Tile leftTile))
                yield return leftTile;
            if(TryGetTile(source.index + new Index(1, 0), out Tile rightTile))
                yield return rightTile;
        }

        return Adjacent().ToList();
    }

    public List<Tile> GetCornerTiles(Tile source)
    {
        IEnumerable<Tile> Corners()
        {
            if(TryGetTile(source.index + new Index(1, 1), out Tile upRightTile))
                yield return upRightTile;
            if(TryGetTile(source.index + new Index(-1, 1), out Tile upLeftTile))
                yield return upLeftTile;
            if(TryGetTile(source.index + new Index(1, -1), out Tile downRightTile))
                yield return downRightTile;
            if(TryGetTile(source.index + new Index(-1, -1), out Tile downLeftTile))
                yield return downLeftTile;
        }

        return Corners().ToList();
    }

    public List<Tile> GetNeighbors(Tile source) => GetAdjacentTiles(source).Concat(GetCornerTiles(source)).ToList();

    public List<Tile> GetAllPathTiles()
    {
        List<Tile> pathTiles = new List<Tile>();
        foreach(List<Tile> row in grid)
        {
            foreach(Tile tile in row)
            {
                if(tile.type == TileType.Path)
                    pathTiles.Add(tile);
            }
        }
        return pathTiles;
    }

    public (Tile bottomLeft, Tile topRight) GetBoundingBox()
    {
        if(grid != null && grid[0] != null && TryGetTile(new Index(0, 0), out Tile bottomLeft) && TryGetTile(new Index(grid.Count - 1, grid[0].Count - 1), out Tile topRight))
            return (bottomLeft, topRight);
        return (null, null);
    }
}