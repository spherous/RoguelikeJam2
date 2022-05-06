using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    public List<Level> levels = new List<Level>();

    private void Start() => LoadLevel(0);

    public void LoadLevel(int index)
    {
        if(index >= levels.Count)
            return;
        
        waveManager.LoadWaves(levels[index]);
    }

    public void CompleteLevel(Level level)
    {
        // start next level
        LoadLevel(levels.IndexOf(level) + 1);
    }
}