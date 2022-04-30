using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isBuildable = false;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Index index {get; private set;}
    public TileType type {get; private set;} = TileType.Path;

    public void SetIndex(Index index) => this.index = index;
    public void SetType(TileType type) 
    {
        this.type = type;
        spriteRenderer.color = type.GetColor();
        gameObject.layer = type.GetLayer();
        if (this.type == TileType.Buildable) isBuildable = true;
    }
}