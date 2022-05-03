using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarlyStartButton : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GroupFader fader;
    private void Awake()
    {
        waveManager.onWaveComplete += OnWaveComplete;
        waveManager.onWaveStart += OnWaveStart;
    }

    private void OnDestroy()
    {
        waveManager.onWaveComplete -= OnWaveComplete;
        waveManager.onWaveStart -= OnWaveStart;
    }

    public void OnClick() => waveManager.StartEarly();

    private void OnWaveStart(Wave wave) => fader.FadeOut();
    private void OnWaveComplete(Wave wave) => fader.FadeIn();
}