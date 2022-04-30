using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Index index {get; private set;}
    public TileType type {get; private set;} = TileType.Path;

    public void SetIndex(Index index) => this.index = index;
    public void SetType(TileType type) => this.type = type;
}