using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] DDoSEnemy DDoSBug;
    [SerializeField] TrojanHorse trojanHorse;
    [SerializeField] private ProcGen procGen;

    public List<Enemy> enemyList = new List<Enemy>();

    [Button]
    public Enemy Spawn(EnemyType type) => SpawnAtTile(type, procGen.spawnPoint);

    public Enemy SpawnAtTile(EnemyType type, Tile spawnPoint)
    {
        Enemy prefab = GetPrefab(type);
        if(prefab == null)
            return null;

        Enemy newSpawn = Instantiate(prefab, spawnPoint.transform.position, transform.rotation).GetComponent<Enemy>();
        enemyList.Add(newSpawn);
        return newSpawn;
    }

    public Enemy GetPrefab(EnemyType type) => type switch {
        EnemyType.DDoSBug => DDoSBug,
        EnemyType.TrojanHorse => trojanHorse,
        _ => null
    };

    public void Remove(Enemy enemy)
    {
        if(enemyList.Contains(enemy))
            enemyList.Remove(enemy);
    }
}