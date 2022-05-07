using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Thread : MonoBehaviour
{
    private LevelManager levelManager;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    public bool available {get; private set;}
    public Sprite availabeSprite;
    public Sprite cooldownSprite;
    public Sprite reservedSprite;
    public Color activeColor;
    public Color inactiveColor;
    public int remainingCooldown;
    private Action onComplete = null;

    private void Awake()
    {
        levelManager = GameManager.FindObjectOfType<LevelManager>();
        levelManager.onLevelComplete += OnLevelComplete;
        available = true;
        text.text = "";
    }
    
    private void OnDestroy() => levelManager.onLevelComplete -= OnLevelComplete;

    private void OnLevelComplete(Level level)
    {
        remainingCooldown = 0;
        onComplete = null;
        TryRefresh();
    }

    public bool Spend()
    {
        if(!available)
            return false;

        available = false;
        image.sprite = cooldownSprite;
        image.color = inactiveColor;
        return true;
    }

    public bool Reserve(int duration, Action onComplete)
    {
        if(!available)
            return false;

        remainingCooldown = duration;
        text.text = $"{remainingCooldown}";
        available = false;
        image.color = activeColor;
        image.sprite = reservedSprite;
        return true;
    }

    public bool TryRefresh()
    {
        if(available)
            return false;
        else if(remainingCooldown == 0)
        {
            Refresh();
            return true;
        }

        remainingCooldown--;
        text.text = $"{remainingCooldown}";
        return false;
    }

    private void Refresh()
    {
        available = true;
        image.sprite = availabeSprite;
        image.color = activeColor;
        text.text = "";
        
        if(onComplete != null)
        {
            onComplete();
            onComplete = null;
        }
    }
}