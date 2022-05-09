using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Card", menuName = "Tower Card")]
public class TowerCard : ScriptableObject, ICard
{
    [SerializeField] public GameObject towerToSpawn;
    [field:SerializeField] public string description {get; set;}
    [field:SerializeField] public CardType type {get; set;} = CardType.Tower;
    [field:SerializeField] public Sprite artwork {get; set;}
    [field:SerializeField] public int threadCost {get; set;}
    [field:SerializeField] public bool singleUse {get; set;}
    [field:SerializeField] public ThreadReserveType threadReserveType {get; set;} = ThreadReserveType.None;
    [field:SerializeField] public ThreadEffectTriggerCondition threadEffectTriggerCondition {get; set;} = ThreadEffectTriggerCondition.OnComplete;
    [field:SerializeField] public int threadUseDuration {get; set;}

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