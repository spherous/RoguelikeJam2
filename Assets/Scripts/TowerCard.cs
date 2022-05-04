using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class TowerCard : ScriptableObject, ICard
{
    [SerializeField] private DummyTower towerToSpawn;
    [field:SerializeField] public string description {get; set;}
    [field:SerializeField] public Sprite artwork {get; set;}
    [field:SerializeField] public int threadCost {get; set;}

    public bool TryPlay(Tile tile)
    {
        if(tile == null || !tile.isBuildable)
            return false;

        ThreadPool threadPool = GameObject.FindObjectOfType<ThreadPool>();

        if(threadPool != null && threadPool.Request(threadCost))
        {
            DummyTower tower = Instantiate(towerToSpawn);
            return true;
        }
        
        return false;
    }
}
