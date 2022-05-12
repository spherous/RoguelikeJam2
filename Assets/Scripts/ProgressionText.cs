using UnityEngine;
using TMPro;
using System;

public class ProgressionText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GroupFader fader;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private BuildMode buildMode;
    [SerializeField] private CardSelection cardSelection;
    public float displayTime;
    private float? hideAtTime;

    private void Awake()
    {
        waveManager.onWaveStart += OnWaveStart;
        waveManager.onWaveComplete += OnWaveComplete;
        levelManager.onLevelStart += OnLevelStart;
        levelManager.onLevelComplete += OnLevelComplete;
        buildMode.buildModeStateChange += BuildModeStateChange;
        cardSelection.cardSelectionStateChange += CardSelectionStateChange;
    }

    private void OnDestroy()
    {
        waveManager.onWaveStart -= OnWaveStart;
        waveManager.onWaveComplete -= OnWaveComplete;
        levelManager.onLevelStart -= OnLevelStart;
        levelManager.onLevelComplete -= OnLevelComplete;
        buildMode.buildModeStateChange -= BuildModeStateChange;
        cardSelection.cardSelectionStateChange -= CardSelectionStateChange;
    }

    private void Update()
    {
        TryHide();
    }

    private void TryHide()
    {
        if(hideAtTime.HasValue && fader.visible && Time.timeSinceLevelLoad >= hideAtTime.Value)
        {
            hideAtTime = null;
            fader.FadeOut();
        }
    }

    public void ForceShow(string textToDisplay)
    {
        if(!fader.visible)
            fader.FadeIn();
        
        hideAtTime = null;
        text.text = textToDisplay;
    }
    public void Hide()
    {
        if(fader.visible)
            fader.FadeOut();
    }

    private void OnWaveComplete(Wave wave)
    {
        TryShow();
        text.text = $"Wave {waveManager.currentWave + 1} Complete";
    }

    private void OnLevelStart(Level level)
    {
        TryShow();
        text.text = $"Level {levelManager.currentLevelIndex + 1} Start";
    }

    private void OnLevelComplete(Level level)
    {
        TryShow();
        text.text = $"Level {levelManager.currentLevelIndex + 1} Complete";
    }

    private void TryShow()
    {
        if(!fader.visible)
        {
            fader.FadeIn();
            hideAtTime = Time.timeSinceLevelLoad + displayTime;
        }
    }

    private void OnWaveStart(Wave wave)
    {
        TryShow();
        text.text = $"Wave {waveManager.currentWave + 1} Start";
    }
    private void BuildModeStateChange(bool buildModeOn)
    {
        if(buildModeOn)
        {
            if (buildMode.movingTower)
            {
                ForceShow("Move A Tower");
                return;
            }
        ForceShow("Build Mode");
        }
        else
            Hide();
        

    }
    private void CardSelectionStateChange(bool selecting, int holdCount)
    {
        if(selecting)
            ForceShow($"Select {holdCount} Cards To Keep");
        else
            Hide();
    }
}