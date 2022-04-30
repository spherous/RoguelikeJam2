using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [ShowInInspector, ReadOnly] public List<List<Tile>> grid = new List<List<Tile>>();

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