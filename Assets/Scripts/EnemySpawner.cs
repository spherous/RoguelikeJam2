using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] DDoSEnemy DDoSBug;
    [SerializeField] TrojanHorse trojanHorse;
    [SerializeField] private ProcGen procGen;

    private int lastUsedSpawnPoint = 0;
    public List<Enemy> enemyList = new List<Enemy>();

    [Button]
    public Enemy Spawn(EnemyType type)
    {
        lastUsedSpawnPoint = (lastUsedSpawnPoint + 1).Mod(procGen.spawnPoints.Count);
        Tile spawnPoint = procGen.spawnPoints[lastUsedSpawnPoint];
        
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