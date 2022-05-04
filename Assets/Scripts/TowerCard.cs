using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class TowerCard : ScriptableObject, ICard
{
    [SerializeField] private GameObject towerToSpawn;
    [field:SerializeField] public string description {get; set;}
    [field:SerializeField] public Sprite artwork {get; set;}
    [field:SerializeField] public int threadCost {get; set;}

    public bool TryPlay(Tile tile)
    {
        if(tile == null || !tile.isBuildable || towerToSpawn == null)
            return false;

        ThreadPool threadPool = GameObject.FindObjectOfType<ThreadPool>();

        if(threadPool != null && threadPool.Request(threadCost))
        {
            ITower tower = Instantiate(towerToSpawn, tile.transform.position, Quaternion.identity, tile.transform).GetComponent<ITower>();
            tile.SetTower(tower);
            return true;
        }
        
        return false;
    }
}
