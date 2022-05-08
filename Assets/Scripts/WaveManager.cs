using System;
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
    public float speedModifier = 0f;
    public float healthModifier = 0f;

    [ShowInInspector] private Level? currentLevel = null;

    private void Update()
    {
        if(!currentLevel.HasValue || currentWave >= currentLevel.Value.waves.Count || levelManager.awaitingPlayerChoice)
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
        currentWave = 0;
        spawnedCount = 0;
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
        onWaveStart?.Invoke(wave);
        spawning = true;
    }

    private void WaveComplete()
    {
        enemies.Clear();
        waitingForCompletion = false;
        timeForWaveStart = Time.timeSinceLevelLoad + waveDelay;
        speedModifier = 0f;
        healthModifier = 0f;

        int waveCountInLevel = currentLevel.Value.waves.Count;
        if(currentWave == waveCountInLevel - 1)
        {
            // level complete
            levelManager.CompleteLevel(currentLevel.Value);
            return;
        }

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
        
        IEnemy newEnemy = spawner.Spawn();
        enemies.Add(((MonoBehaviour)newEnemy).gameObject);

        newEnemy.AdjustSpeed(speedModifier);
        newEnemy.AdjustHealth(healthModifier);
        newEnemy.onHealthChanged += EnemyHealthChanged;
        
        if(spawnedCount >= wave.enemyCount)
            EndWave();
    }

    private void EnemyHealthChanged(IHealth changed, float oldHP, float newHP, float percent)
    {
        if(newHP <= 0)
        {
            enemies.Remove(((MonoBehaviour)changed).gameObject);
            changed.onHealthChanged -= EnemyHealthChanged;
        }
    }

    public void AdjustSpeed(float amount)
    {
        speedModifier += amount;
        foreach(GameObject enemyGO in enemies)
        {
            if(enemyGO.TryGetComponent<IEnemy>(out IEnemy e))
                e.AdjustSpeed(amount);
        }
    }

    public void AdjustHealth(float percent)
    {
        healthModifier += percent;
        foreach(GameObject enemyGO in enemies)
        {
            if(enemyGO.TryGetComponent<IEnemy>(out IEnemy e))
                e.AdjustHealth(percent);
        }
    }
}