using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackKnight : Enemy
{
    [SerializeField] private AudioSource audioSource;
    public AudioClip spawnClip;

    WaveManager waveManager;
    private float ddosBugSpawnAtTime = 0;
    public float delayBetweenDDoSBugs = 0.666f;
    
    private void Awake()
    {
        waveManager = FindObjectOfType<WaveManager>();
    }

    protected override void Start()
    {
        onHealthChanged += SpawnTrojanHorseOnHit;
        base.Start();
        audioSource.PlayOneShot(spawnClip);
    }

    public void SpawnTrojanHorseOnHit(IHealth changed, float oldHP, float newHP, float percent)
    {
        if(newHP < oldHP)
            SpawnMinionNearby(EnemyType.TrojanHorse);
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad > ddosBugSpawnAtTime)
            SpawnMinionNearby(EnemyType.DDoSBug);
    }

    private void SpawnMinionNearby(EnemyType type)
    {
        Enemy newMinion = waveManager.SpawnEnemy(type, currentTile);
        newMinion.transform.position += (Vector3)UnityEngine.Random.insideUnitCircle;
        ddosBugSpawnAtTime = Time.timeSinceLevelLoad + delayBetweenDDoSBugs;
        newMinion.pathingToNode = pathingToNode;
    }
}