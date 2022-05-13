using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [ShowInInspector, ReadOnly] public bool isBuildable => this.type == TileType.Buildable && tower == null;
    public ITower tower {get; private set;} = null;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [ShowInInspector, ReadOnly] public Index index {get; private set;}
    [ShowInInspector, ReadOnly] public TileType type {get; private set;} = TileType.Path;

    [SerializeField] private Sprite pathSprite;
    [SerializeField] private Sprite buildableSprite;

    public Sprite pathPillarSprite;
    public Sprite pathEndCapSprite;
    public Sprite pathLengthSprite;
    public Sprite pathCornerSprite;
    public Sprite pathTJunctionSprite;
    public Sprite pathCrossSprite;

    public List<Sprite> pillars = new List<Sprite>();
    public List<Sprite> lengths = new List<Sprite>();
    public List<Sprite> corners = new List<Sprite>();
    public List<Sprite> endCap = new List<Sprite>();
    public Sprite tIntersection;
    public Sprite fourWayIntersection;

    [SerializeField] private Sprite spawnPointSprite;
    [SerializeField] private Sprite homeSprite;

    public List<int> rotations = new List<int>();

    GridGenerator gridGenerator;

    private void Awake() => gridGenerator = GameObject.FindObjectOfType<GridGenerator>();

    public void SetTower(ITower toSet)
    {
        toSet.location = index;
        tower = toSet;
    }
    public void SetIndex(Index index) => this.index = index;
    public void SetType(TileType type) 
    {
        this.type = type;
        spriteRenderer.sprite = GetSprite(type);
        gameObject.layer = type.GetLayer();
    }

    public Sprite GetSprite(TileType type) => type switch{
        TileType.Path => pathSprite,
        TileType.Buildable => buildableSprite,
        TileType.EnemySpawnPoint => spawnPointSprite,
        TileType.Home => homeSprite,
        _ => buildableSprite
    };
    public void DestroyTower()
    {
        if(tower == null || ((MonoBehaviour)tower).gameObject == null)
            return;

        Destroy(((MonoBehaviour)tower).gameObject);
        tower = null;
    }

    public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;

    public void UpdateSprite()
    {
        if(type == TileType.Buildable)
            UpdateBuildable();
        else if(type == TileType.Path)
            UpdatePath();
    }
    
    private void UpdatePath()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
        var adjacentTiles = gridGenerator.GetAdjacentTiles(this);
        int pathCount = adjacentTiles.Count(t => t != null && t.type == TileType.Path);
        if(pathCount == 0)
        {
            SetSprite(pathPillarSprite);
            return;
        }
        else if(pathCount == 1)
        {
            SetSprite(pathEndCapSprite);
            if(adjacentTiles[0] != null && adjacentTiles[0].type == TileType.Path)
                transform.rotation = Quaternion.Euler(0, 0, rotations[3]);
            else if(adjacentTiles[2] != null && adjacentTiles[2].type == TileType.Path)
                transform.rotation = Quaternion.Euler(0, 0, rotations[2]);
            else if(adjacentTiles[1] != null && adjacentTiles[1].type == TileType.Path)
                transform.rotation = Quaternion.Euler(0, 0, rotations[1]);
            return;
        }
        else if(pathCount == 2)
        {
            // corner or wall length
            if(adjacentTiles[0] != null && adjacentTiles[0].type == TileType.Path && adjacentTiles[2] != null && adjacentTiles[2].type == TileType.Path)
            {
                transform.rotation = Quaternion.Euler(0, 0, rotations[1]);
                SetSprite(pathCornerSprite);
            }
            else if(adjacentTiles[0] != null && adjacentTiles[0].type == TileType.Path && adjacentTiles[3] != null && adjacentTiles[3].type == TileType.Path)
            {
                SetSprite(pathCornerSprite);
                transform.rotation = Quaternion.Euler(0, 0, rotations[2]);
            }
            else if(adjacentTiles[1] != null && adjacentTiles[1].type == TileType.Path && adjacentTiles[3] != null && adjacentTiles[3].type == TileType.Path)
            {
                SetSprite(pathCornerSprite);
                transform.rotation = Quaternion.Euler(0, 0, rotations[3]);
            }
            else if(adjacentTiles[1] != null && adjacentTiles[1].type == TileType.Path && adjacentTiles[2] != null && adjacentTiles[2].type == TileType.Path)
                SetSprite(pathCornerSprite);
            else if(adjacentTiles[0] != null && adjacentTiles[0].type == TileType.Path && adjacentTiles[1] != null && adjacentTiles[1].type == TileType.Path)
            {
                SetSprite(pathLengthSprite);
                transform.rotation = Quaternion.Euler(0, 0, rotations[1]);
            }
            else if(adjacentTiles[2] != null && adjacentTiles[2].type == TileType.Path && adjacentTiles[3] != null && adjacentTiles[3].type == TileType.Path)
                SetSprite(pathLengthSprite);
        }
        else if(pathCount == 3)
        {
            SetSprite(pathTJunctionSprite);
            if(adjacentTiles[0] == null || adjacentTiles[0].type != TileType.Path)
                transform.rotation = Quaternion.Euler(0, 0, rotations[1]);
            else if(adjacentTiles[1] == null || adjacentTiles[1].type != TileType.Path)
                transform.rotation = Quaternion.Euler(0, 0, rotations[3]);
            else if(adjacentTiles[3] == null || adjacentTiles[3].type != TileType.Path)
                transform.rotation = Quaternion.Euler(0, 0, rotations[2]);
        }
        else if(pathCount == 4)
        {
            SetSprite(pathCrossSprite);
            // transform.rotation = Quaternion.Euler(0, 0, rotations.ChooseRandom());
        }
    }

    private void UpdateBuildable()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
        var adjacentTiles = gridGenerator.GetAdjacentTiles(this);
        int buildableCount = adjacentTiles.Count(t => t != null && t.type == TileType.Buildable);
        if(buildableCount == 0)
        {
            SetSprite(pillars.ChooseRandom());
            transform.rotation = Quaternion.Euler(0, 0, rotations.ChooseRandom());
        }
        else if(buildableCount == 1)
        {
            SetSprite(endCap.ChooseRandom());
            if(adjacentTiles[0] != null && adjacentTiles[0].type == TileType.Buildable)
                transform.rotation = Quaternion.Euler(0, 0, rotations[2]);
            else if(adjacentTiles[2] != null && adjacentTiles[2].type == TileType.Buildable)
                transform.rotation = Quaternion.Euler(0, 0, rotations[1]);
            else if(adjacentTiles[3] != null && adjacentTiles[3].type == TileType.Buildable)
                transform.rotation = Quaternion.Euler(0, 0, rotations[3]);
        }
        else if(buildableCount == 2)
        {
            // corner or wall length
            if(adjacentTiles[0] != null && adjacentTiles[0].type == TileType.Buildable && adjacentTiles[2] != null && adjacentTiles[2].type == TileType.Buildable)
            {
                transform.rotation = Quaternion.Euler(0, 0, rotations[2]);
                SetSprite(corners.ChooseRandom());
            }
            else if(adjacentTiles[0] != null && adjacentTiles[0].type == TileType.Buildable && adjacentTiles[3] != null && adjacentTiles[3].type == TileType.Buildable)
            {
                SetSprite(corners.ChooseRandom());
                transform.rotation = Quaternion.Euler(0, 0, rotations[3]);
            }
            else if(adjacentTiles[1] != null && adjacentTiles[1].type == TileType.Buildable && adjacentTiles[2] != null && adjacentTiles[2].type == TileType.Buildable)
            {
                // Debug.Log(index);
                SetSprite(corners.ChooseRandom());
                transform.rotation = Quaternion.Euler(0, 0, rotations[1]);
            }
            else if(adjacentTiles[1] != null && adjacentTiles[1].type == TileType.Buildable && adjacentTiles[3] != null && adjacentTiles[3].type == TileType.Buildable)
                SetSprite(corners.ChooseRandom());
            else if(adjacentTiles[0] != null && adjacentTiles[0].type == TileType.Buildable && adjacentTiles[1] != null && adjacentTiles[1].type == TileType.Buildable)
                SetSprite(lengths.ChooseRandom());
            else if(adjacentTiles[2] != null && adjacentTiles[2].type == TileType.Buildable && adjacentTiles[3] != null && adjacentTiles[3].type == TileType.Buildable)
            {
                transform.rotation = Quaternion.Euler(0, 0, rotations[3]);
                SetSprite(lengths.ChooseRandom());
            }
        }
        else if(buildableCount == 3)
        {
            SetSprite(tIntersection);
            if(adjacentTiles[0] == null || adjacentTiles[0].type != TileType.Buildable)
                transform.rotation = Quaternion.Euler(0, 0, rotations[1]);
            else if(adjacentTiles[1] == null || adjacentTiles[1].type != TileType.Buildable)
                transform.rotation = Quaternion.Euler(0, 0, rotations[3]);
            else if(adjacentTiles[3] == null || adjacentTiles[3].type != TileType.Buildable)
                transform.rotation = Quaternion.Euler(0, 0, rotations[2]);
        }
        else if(buildableCount == 4)
        {
            SetSprite(fourWayIntersection);
            transform.rotation = Quaternion.Euler(0, 0, rotations.ChooseRandom());
        }
    }
}