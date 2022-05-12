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
    private GameManager gameManager;
    [SerializeField] private Thread threadPrefab;
    [SerializeField] private GroupFader fader;
    public List<Thread> threads {get; private set;} = new List<Thread>();
    public int availableThreads => GetAvailableThreads().Count();
    public int unavailableThreds => GetUnavailableThreads().Count();
    public IEnumerable<Thread> GetAvailableThreads() => threads.Where(t => t.available);
    public IEnumerable<Thread> GetUnavailableThreads() => threads.Where(t => !t.available);

    private void Awake()
    {
        waveManager = GameObject.FindObjectOfType<WaveManager>();
        waveManager.onWaveComplete += OnWaveComplete;
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.onLevelStart += OnLevelStart;
        gameManager = GameObject.FindObjectOfType<GameManager>();
        gameManager.onHealthChanged += OnHealthChanged;
    }

    private void OnDestroy()
    {
        waveManager.onWaveComplete -= OnWaveComplete;
        levelManager.onLevelStart -= OnLevelStart;
        gameManager.onHealthChanged -= OnHealthChanged;
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

    public void ManualRefresh()
    {
        foreach(Thread thread in GetUnavailableThreads())
        {
            if(thread.remainingCooldown == 0) // refresh the 1st thread that isn't reserved
            {
                thread.TryRefresh();
                return;
            }
        }
    }

    public void GainThreadAtEndOfFrame()
    {
        StartCoroutine(ThreadGainRoutine());
    }

    public IEnumerator ThreadGainRoutine()
    {
        yield return new WaitForEndOfFrame();
        IncreaseThreadCount(1);
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
        
        List<Thread> threadsToReserve = GetAvailableThreads().Take(amount).ToList();
        for(int i = 0; i < amount; i++)
        {
            Thread thread = threadsToReserve.ElementAt(i);
            Action onCompleteModified = (reserveType == ThreadReserveType.Single && i == 0) || reserveType == ThreadReserveType.All ? onComplete : null;
            thread.Reserve(duration, onCompleteModified, triggerCondition);
        }
        
        return true;
    }

    private void OnHealthChanged(IHealth changed, float oldHP, float newHP, float percent)
    {
        if(newHP == 0)
            Hide();
    }

    public void Hide() => fader.FadeOut();
}