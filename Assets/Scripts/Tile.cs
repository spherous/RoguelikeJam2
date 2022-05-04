using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isBuildable => this.type == TileType.Buildable && tower == null;
    public ITower tower {get; private set;} = null;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [ShowInInspector, ReadOnly] public Index index {get; private set;}
    [ShowInInspector, ReadOnly] public TileType type {get; private set;} = TileType.Path;

    public void SetTower(ITower toSet) => tower = toSet;
    public void SetIndex(Index index) => this.index = index;
    public void SetType(TileType type) 
    {
        this.type = type;
        spriteRenderer.color = type.GetColor();
        gameObject.layer = type.GetLayer();
    }
}