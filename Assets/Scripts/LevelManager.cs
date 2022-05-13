using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public delegate void OnLevelChanged(Level level);
    public OnLevelChanged onLevelStart;
    public OnLevelChanged onLevelComplete;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private PickNewCard pickNewCard;
    [SerializeField] private WinScreen winScreen;
    [SerializeField] private GameManager gameManager;
    SceneTransition sceneTransition;
    [SerializeField] private SceneTransition sceneTransitionPrefab;
    ProcGen procGen;
    public List<Level> levels = new List<Level>();
    public int currentLevelIndex {get; private set;} = 0;
    public Level currentLevel {get; private set;}
    public bool awaitingPlayerChoice {get; private set;} = false;

    private void Start()
    {
        procGen = GameObject.FindObjectOfType<ProcGen>();
        sceneTransition = GameObject.FindObjectOfType<SceneTransition>();
        LoadLevel(currentLevelIndex);
    } 

    public void LoadLevel(int levelIndex)
    {
        if(levelIndex >= levels.Count)
            return;
        
        foreach(List<Tile> row in gridGenerator.grid)
        {
            foreach(Tile tile in row)
            {
                if(tile != null && tile.tower != null)
                    tile.DestroyTower();
            }
        }

        currentLevelIndex = levelIndex;
        currentLevel = levels[currentLevelIndex];

        procGen.Generate(currentLevel.rows, currentLevel.cols);

        waveManager.LoadWaves(currentLevel);
        onLevelStart?.Invoke(currentLevel);
    }

    public void CompleteLevel(Level level)
    {
        int lastLevel = levels.IndexOf(level);
        if(lastLevel + 1 >= levels.Count)
        {
            if(gameManager.currentHP > 0)
                Win();
            return;
        }

        gameManager.HealToFull();
        onLevelComplete?.Invoke(level);
        awaitingPlayerChoice = true;
        currentLevelIndex = lastLevel + 1;
        pickNewCard.OfferChoice(() => {
            awaitingPlayerChoice = false;
            if(sceneTransition == null)
                sceneTransition = Instantiate(sceneTransitionPrefab);
            sceneTransition.LoadingScreen(() => LoadLevel(currentLevelIndex));
        });
    }

    private void Win() => winScreen.Display();
}