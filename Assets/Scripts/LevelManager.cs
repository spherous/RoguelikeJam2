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
    public List<Level> levels = new List<Level>();
    public int currentLevelIndex {get; private set;} = 0;
    public Level currentLevel {get; private set;}
    public bool awaitingPlayerChoice {get; private set;} = false;

    private void Start() => LoadLevel(currentLevelIndex);

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

        waveManager.LoadWaves(currentLevel);
        onLevelStart?.Invoke(currentLevel);
    }

    public void CompleteLevel(Level level)
    {
        onLevelComplete?.Invoke(level);
        currentLevelIndex = levels.IndexOf(level) + 1;
        awaitingPlayerChoice = true;
        pickNewCard.OfferChoice(() => {
            awaitingPlayerChoice = false;
            LoadLevel(currentLevelIndex);
        });
    }
}