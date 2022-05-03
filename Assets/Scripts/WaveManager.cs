using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public delegate void OnWaveChange(Wave wave);
    public OnWaveChange onWaveStart;
    public OnWaveChange onWaveComplete;
    public OnWaveChange onWaveEnd;

    [SerializeField] private EnemySpawner spawner;
    public List<Wave> waves = new List<Wave>();
    [ReadOnly] public List<GameObject> enemies = new List<GameObject>();

    public float waveDelay = 30f;
    bool spawning = false;

    private int currentWave = 0;
    private bool waitingForCompletion = false;

    // We need to maintain a separate counter, because the player may still be killing enemies from the previous wave when we stat spawning the next wave.
    private int spawnedCount = 0;
    private float timeForNextSpawn;
    private float timeForWaveStart;

    private void Start()
    {
        timeForWaveStart = Time.timeSinceLevelLoad + waveDelay;
    }

    private void Update()
    {
        if(currentWave >= waves.Count)
            return;

        if(waitingForCompletion && EnemiesRemaining() == 0)
            WaveComplete();
        else if(!waitingForCompletion && !spawning && Time.timeSinceLevelLoad >= timeForWaveStart)
            StartWave();
        else if(!waitingForCompletion && spawning && Time.timeSinceLevelLoad >= timeForNextSpawn)
            Spawn();
    }

    private int EnemiesRemaining() => enemies.Where(enemy => enemy != null).Count();

    public void StartEarly()
    {
        if(waitingForCompletion || spawning)
            return;
            
        StartWave();
    }

    private void StartWave()
    {
        timeForNextSpawn = Time.timeSinceLevelLoad + waves[currentWave].spawnInterval;

        if(currentWave >= waves.Count)
        {
            // level complete
            return;
        }

        onWaveStart?.Invoke(waves[currentWave]);
        spawning = true;
    }

    private void WaveComplete()
    {
        enemies.Clear();
        waitingForCompletion = false;
        timeForWaveStart = Time.timeSinceLevelLoad + waveDelay;
        onWaveComplete?.Invoke(waves[currentWave]);
        currentWave++;
    }

    private void EndWave()
    {   
        spawning = false;
        onWaveEnd?.Invoke(waves[currentWave]);
        waitingForCompletion = true;
        spawnedCount = 0;
    }

    private void Spawn()
    {
        timeForNextSpawn = Time.timeSinceLevelLoad + waves[currentWave].spawnInterval;
        spawnedCount++;
        enemies.Add(spawner.Spawn());
        if(spawnedCount >= waves[currentWave].enemyCount)
            EndWave();
    }
}