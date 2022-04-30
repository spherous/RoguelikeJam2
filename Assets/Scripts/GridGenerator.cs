using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [ShowInInspector, ReadOnly] public List<List<Tile>> grid = new List<List<Tile>>();

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
}