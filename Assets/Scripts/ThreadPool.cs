using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ThreadPool : MonoBehaviour
{
    [SerializeField] private Thread threadPrefab;
    public List<Thread> threads {get; private set;} = new List<Thread>();
    public int availableThreads => GetAvailableThreads().Count();

    private IEnumerable<Thread> GetAvailableThreads() => threads.Where(t => t.available);

    public void IncreaseThreadCount(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            Thread thread = Instantiate(threadPrefab, transform);
            thread.Toggle(true);
            threads.Add(thread);
        }
    }

    [Button]
    public void Refresh()
    {
        foreach(Thread thread in threads)
            thread.Toggle(true);
    }

    [Button]
    public bool Request(int amount)
    {
        if(amount > availableThreads)
            return false;

        foreach(Thread thread in GetAvailableThreads().Take(amount))
            thread.Toggle(false);

        return true;
    }
}