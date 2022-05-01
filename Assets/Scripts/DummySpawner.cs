using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawner : MonoBehaviour
{
    EnemyList enemyList;
    [SerializeField] GameObject enemy;
    private float spawnTime;
    public float spawnRate;

    void Start()
    {
        spawnTime = Time.timeSinceLevelLoad + spawnRate;
        enemyList = GameObject.Find("GameManager").GetComponent<EnemyList>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad >= spawnTime)
        {
            spawnTime = Time.timeSinceLevelLoad + spawnRate;
            Spawn();
        }
    }
    void Spawn()
    {
        GameObject newSpawn = Instantiate(enemy, transform.position, transform.rotation);
        enemyList.enemyList.Add(newSpawn);
    }
}
