using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Card", menuName = "Tower Card")]
public class TowerCard : ScriptableObject, ICard
{
    // [SerializeField] public GameObject towerToSpawn;
    public TowerType towerToSpawn;
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
        if(tile == null || !tile.isBuildable)
            return false;

        ThreadPool threadPool = GameObject.FindObjectOfType<ThreadPool>();
        TowerManager towerManager = GameObject.FindObjectOfType<TowerManager>();

        if(towerManager != null && threadPool != null && TrySpendThreads(threadPool))
        {
            ITower tower = towerManager.SpawnTower(towerToSpawn, tile);
            // ITower tower = Instantiate(towerToSpawn, tile.transform.position, Quaternion.identity, tile.transform).GetComponent<ITower>();
            tile.SetTower(tower);
            return true;
        }
        
        return false;
    }
    
    public bool TrySpendThreads(ThreadPool threadPool) => threadPool.Request(threadCost);
}