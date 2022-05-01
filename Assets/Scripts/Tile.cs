using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isBuildable = false;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [ShowInInspector, ReadOnly] public Index index {get; private set;}
    [ShowInInspector, ReadOnly] public TileType type {get; private set;} = TileType.Path;

    public void SetIndex(Index index) => this.index = index;
    public void SetType(TileType type) 
    {
        this.type = type;
        spriteRenderer.color = type.GetColor();
        gameObject.layer = type.GetLayer();
        if (this.type == TileType.Buildable) isBuildable = true;
    }
}