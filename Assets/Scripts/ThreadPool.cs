using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public enum ThreadReserveType {None = 0, Single = 1, All = 2}
public enum ThreadEffectTriggerCondition {EveryTurn = 0, OnComplete = 1}
public class ThreadPool : MonoBehaviour
{
    private LevelManager levelManager;
    private WaveManager waveManager;
    [SerializeField] private Thread threadPrefab;
    public List<Thread> threads {get; private set;} = new List<Thread>();
    public int availableThreads => GetAvailableThreads().Count();

    private IEnumerable<Thread> GetAvailableThreads() => threads.Where(t => t.available);

    private void Awake()
    {
        waveManager = GameObject.FindObjectOfType<WaveManager>();
        waveManager.onWaveComplete += OnWaveComplete;
        levelManager = GameManager.FindObjectOfType<LevelManager>();
        levelManager.onLevelStart += OnLevelStart;
    }

    private void OnDestroy()
    {
        waveManager.onWaveComplete -= OnWaveComplete;
        levelManager.onLevelStart -= OnLevelStart;
    }

    private void OnLevelStart(Level level)
    {
        Reset();
        Refresh();
    } 
    private void OnWaveComplete(Wave endingWave) => Refresh();

    public void IncreaseThreadCount(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            Thread thread = Instantiate(threadPrefab, transform);
            threads.Add(thread);
        }
    }

    [Button]
    public void Refresh()
    {
        foreach(Thread thread in threads)
            thread.TryRefresh();
    }

    [Button]
    public bool Request(int amount)
    {
        if(amount > availableThreads)
            return false;

        foreach(Thread thread in GetAvailableThreads().Take(amount))
            thread.Spend();

        return true;
    }

    public void Reset()
    {
        if(threads.Count > 3)
        {
            for(int i = threads.Count - 1; i >= 3; i--)
            {
                Destroy(threads[i].gameObject);
                threads.RemoveAt(i);
            }
        }
    }

    public bool RequestReserve(int amount, int duration, Action onComplete, ThreadReserveType reserveType = ThreadReserveType.Single, ThreadEffectTriggerCondition triggerCondition = ThreadEffectTriggerCondition.OnComplete)
    {
        if(amount > availableThreads)
            return false;
        else if(reserveType == ThreadReserveType.None)
            return Request(amount);
        
        IEnumerable<Thread> threadsToReserve = GetAvailableThreads().Take(amount);
        for(int i = 0; i < amount; i++)
        {
            Thread thread = threadsToReserve.ElementAt(i);
            Action onCompleteModified = (reserveType == ThreadReserveType.Single && i == 0) || reserveType == ThreadReserveType.All ? onComplete : null;
            thread.Reserve(duration, onCompleteModified, triggerCondition);
        }
        
        return true;
    }
}