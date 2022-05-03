using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildTimePanel : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GroupFader fader;
    [SerializeField] private TextMeshProUGUI timeText;
    bool ticking = false;
    float timeRemaining = 0;

    private void Awake()
    {
        waveManager.onWaveComplete += OnWaveComplete;
        waveManager.onWaveStart += OnWaveStart;
    }

    private void Start()
    {
        timeRemaining = waveManager.waveDelay;
        ticking = true;
    }

    private void OnDestroy()
    {
        waveManager.onWaveComplete -= OnWaveComplete;
        waveManager.onWaveStart -= OnWaveStart;
    }

    private void Update()
    {
        if(ticking)
        {
            timeRemaining -= Time.deltaTime;
            timeText.text = TimeSpan.FromSeconds(timeRemaining).ToString(timeRemaining.GetStringFromSeconds());
        }    
    }

    public void OnClick() => waveManager.StartEarly();

    private void OnWaveStart(Wave wave)
    {
        ticking = false;
        fader.FadeOut();  
    } 
    private void OnWaveComplete(Wave wave)
    {
        timeRemaining = waveManager.waveDelay;
        ticking = true;
        fader.FadeIn();
    } 
}