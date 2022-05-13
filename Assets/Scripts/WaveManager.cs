using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class WaveManager : SerializedMonoBehaviour
{
    public delegate void OnWaveChange(Wave wave);
    public OnWaveChange onWaveStart;
    public OnWaveChange onWaveComplete;
    public OnWaveChange onWaveEnd;

    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CardSelection cardSelection;
    private ThreadPool threadPool;

    [ReadOnly] public List<Enemy> enemies = new List<Enemy>();

    public float waveDelay = 30f;
    bool spawning = false;

    public int currentWave {get; private set;} = 0;
    private bool waitingForCompletion = false;

    [OdinSerialize, ReadOnly] private List<(EnemyType, int)> spawnedCount = new List<(EnemyType, int)>();
    private float timeForNextSpawn;
    private float timeForWaveStart;
    public float speedModifier = 0f;
    public float healthModifier = 0f;

    [ShowInInspector] private Level? currentLevel = null;

    private void Start() => threadPool = GameObject.FindObjectOfType<ThreadPool>();

    private void Update()
    {
        if(!currentLevel.HasValue || currentWave >= currentLevel.Value.waves.Count || levelManager.awaitingPlayerChoice || gameManager.gameOver)
            return;

        if(waitingForCompletion && EnemiesRemaining() == 0)
            WaveComplete();
        else if(!waitingForCompletion && !spawning && Time.timeSinceLevelLoad >= timeForWaveStart)
            StartWave();
        else if(!waitingForCompletion && spawning && !cardSelection.selecting && Time.timeSinceLevelLoad >= timeForNextSpawn)
            SpawnWave();
    }

    public void LoadWaves(Level level)
    {
        currentWave = 0;
        spawnedCount.Clear();
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
        if(gameManager.gameOver)
            return;

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

        if(currentLevel.Value.waves[currentWave].addsThread && threadPool != null)
            threadPool.IncreaseThreadCount(1);
        onWaveComplete?.Invoke(currentLevel.Value.waves[currentWave]);
        currentWave++;
    }

    private void EndWave()
    {   
        spawning = false;
        onWaveEnd?.Invoke(currentLevel.Value.waves[currentWave]);
        waitingForCompletion = true;
        spawnedCount.Clear();
    }

    private void SpawnWave()
    {
        Wave wave = currentLevel.Value.waves[currentWave];
        timeForNextSpawn = Time.timeSinceLevelLoad + wave.spawnInterval;
        
        List<EnemyType> availableTypes = new List<EnemyType>();
        foreach(Group group in wave.enemyGroups)
        {
            if(!spawnedCount.Any(c => c.Item1 == group.type))
            {
                spawnedCount.Add((group.type, 0));
                availableTypes.Add(group.type);
                continue;
            }

            foreach(var (type, count) in spawnedCount)
            {
                if(type == group.type && count < group.count && !availableTypes.Contains(type))
                {
                    availableTypes.Add(group.type);
                    break;
                }
            }
        }

        EnemyType toSpawn = availableTypes.ChooseRandom();
        SpawnEnemy(toSpawn);

        for(int i = 0; i < spawnedCount.Count; i++)
        {
            (EnemyType type, int spawnCount) count = spawnedCount[i];
            spawnedCount[i] = (count.type, count.spawnCount + 1);
        }

        bool waveFinishedSpawning = true;

        for(int i = 0; i < wave.enemyGroups.Count; i++)
        {
            (EnemyType type, int spawnCount) count = spawnedCount.Find(c => c.Item1 == wave.enemyGroups[i].type);
            waveFinishedSpawning = count.spawnCount >= wave.enemyGroups[i].count;
            if(!waveFinishedSpawning)
                return;
        }
        
        if(waveFinishedSpawning)
            EndWave();
    }

    public Enemy SpawnEnemy(EnemyType type, Tile tile = null)
    {
        Enemy newEnemy = tile == null ? spawner.Spawn(type) : spawner.SpawnAtTile(type, tile);
        enemies.Add(newEnemy);

        newEnemy.AdjustSpeed(speedModifier);
        newEnemy.AdjustHealth(healthModifier);
        newEnemy.onHealthChanged += EnemyHealthChanged;
        return newEnemy;
    }

    private void EnemyHealthChanged(IHealth changed, float oldHP, float newHP, float percent)
    {
        if(newHP <= 0 && changed is Enemy enemy)
        {
            enemies.Remove(enemy);
            changed.onHealthChanged -= EnemyHealthChanged;
        }
    }

    public void AdjustSpeed(float amount)
    {
        speedModifier += amount;
        foreach(Enemy enemy in enemies)
        {
            if(enemy.TryGetComponent<Enemy>(out Enemy e))
                e.AdjustSpeed(amount);
        }
    }

    public void AdjustHealth(float percent)
    {
        healthModifier += percent;
        foreach(Enemy enemy in enemies)
        {
            if(enemy.TryGetComponent<Enemy>(out Enemy e))
                e.AdjustHealth(percent);
        }
    }
}

// public class EnemyTypeComparer : IEqualityComparer<(EnemyType, int)>
// {
//     public bool Equals(Group x, (EnemyType, int) y) => x.Item1 == y.Item1;

//     public int GetHashCode((EnemyType, int) obj) => obj.Item1.GetHashCode();
// }