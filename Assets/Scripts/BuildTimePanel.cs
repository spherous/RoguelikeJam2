using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BuildTimePanel : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GroupFader fader;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private Image buttonRenderer;
    [SerializeField] private Sprite clickedSprite;
    bool ticking = false;
    float timeRemaining = 0;

    private void Awake()
    {
        waveManager.onWaveComplete += OnWaveComplete;
        waveManager.onWaveStart += OnWaveStart;
        levelManager.onLevelStart += OnLevelStart;
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

    private void OnLevelStart(Level level)
    {
        if(!fader.visible)
            fader.FadeIn();
        director.Play();
        timeRemaining = waveManager.waveDelay;
        ticking = true;
    }

    private void OnWaveStart(Wave wave)
    {
        director.Stop();
        ticking = false;
        fader.FadeOut();  
    } 
    private void OnWaveComplete(Wave wave)
    {
        timeRemaining = waveManager.waveDelay;
        ticking = true;
        if(!fader.visible)
            fader.FadeIn();
        director.Play();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        director.Stop();
        buttonRenderer.sprite = clickedSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        waveManager.StartEarly();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        director.Play();
    }
}