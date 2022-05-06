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

    [SerializeField] private Sprite pathSprite;
    [SerializeField] private Sprite buildableSprite;

    public void SetTower(ITower toSet) => tower = toSet;
    public void SetIndex(Index index) => this.index = index;
    public void SetType(TileType type) 
    {
        this.type = type;
        spriteRenderer.sprite = GetSprite(type);
        spriteRenderer.color = type.GetColor();
        gameObject.layer = type.GetLayer();
    }

    public Sprite GetSprite(TileType type) => type switch{
        TileType.Path => pathSprite,
        TileType.Buildable => buildableSprite,
        _ => buildableSprite
    };
    public void DestroyTower()
    {
        if(tower == null || ((MonoBehaviour)tower).gameObject == null)
            return;

        Debug.Log("Todo: Destroy towers");
        
        // Destroy(((MonoBehaviour)tower).gameObject);
    }
}