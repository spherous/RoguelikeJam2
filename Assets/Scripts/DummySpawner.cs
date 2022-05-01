using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DummySpawner : MonoBehaviour
{
    EnemyList enemyList;
    [SerializeField] GameObject enemy;
    [SerializeField] private ProcGen procGen;
    private float spawnTime;
    public bool spawnOverTime = true;
    public float spawnRate;

    private int lastUsedSpawnPoint = 0;

    void Start()
    {
        spawnTime = Time.timeSinceLevelLoad + spawnRate;
        enemyList = GameObject.FindObjectOfType<EnemyList>();
    }

    void Update()
    {
        if(!spawnOverTime) 
            return;

        if(Time.timeSinceLevelLoad >= spawnTime)
            Spawn();
    }

    [Button]
    void Spawn()
    {
        spawnTime = Time.timeSinceLevelLoad + spawnRate;
        lastUsedSpawnPoint = (lastUsedSpawnPoint + 1).Mod(procGen.spawnPoints.Count);
        Tile spawnPoint = procGen.spawnPoints[lastUsedSpawnPoint];
        GameObject newSpawn = Instantiate(enemy, spawnPoint.transform.position, transform.rotation);
        enemyList.enemyList.Add(newSpawn);
    }
}