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
    [SerializeField] private LevelManager levelManager;

    [ReadOnly] public List<GameObject> enemies = new List<GameObject>();

    public float waveDelay = 30f;
    bool spawning = false;

    public int currentWave {get; private set;} = 0;
    private bool waitingForCompletion = false;

    private int spawnedCount = 0;
    private float timeForNextSpawn;
    private float timeForWaveStart;

    [ShowInInspector] private Level? currentLevel = null;

    private void Update()
    {
        if(!currentLevel.HasValue || currentWave >= currentLevel.Value.waves.Count)
            return;

        if(waitingForCompletion && EnemiesRemaining() == 0)
            WaveComplete();
        else if(!waitingForCompletion && !spawning && Time.timeSinceLevelLoad >= timeForWaveStart)
            StartWave();
        else if(!waitingForCompletion && spawning && Time.timeSinceLevelLoad >= timeForNextSpawn)
            Spawn();
    }

    public void LoadWaves(Level level)
    {
        currentLevel = level;
        timeForWaveStart = Time.timeSinceLevelLoad + waveDelay;
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
        int waveCountInLevel = currentLevel.Value.waves.Count;
        if(currentWave >= waveCountInLevel)
            return;

        Wave wave = currentLevel.Value.waves[currentWave];
        timeForNextSpawn = Time.timeSinceLevelLoad + wave.spawnInterval;

        if(currentWave >= waveCountInLevel)
        {
            // level complete
            levelManager.CompleteLevel(currentLevel.Value);
            return;
        }

        onWaveStart?.Invoke(wave);
        spawning = true;
    }

    private void WaveComplete()
    {
        enemies.Clear();
        waitingForCompletion = false;
        timeForWaveStart = Time.timeSinceLevelLoad + waveDelay;
        onWaveComplete?.Invoke(currentLevel.Value.waves[currentWave]);
        currentWave++;
    }

    private void EndWave()
    {   
        spawning = false;
        onWaveEnd?.Invoke(currentLevel.Value.waves[currentWave]);
        waitingForCompletion = true;
        spawnedCount = 0;
    }

    private void Spawn()
    {
        Wave wave = currentLevel.Value.waves[currentWave];
        timeForNextSpawn = Time.timeSinceLevelLoad + wave.spawnInterval;
        spawnedCount++;
        enemies.Add(spawner.Spawn());
        if(spawnedCount >= wave.enemyCount)
            EndWave();
    }
}