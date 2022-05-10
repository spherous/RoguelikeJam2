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
    public int remainingCooldown;
    private Action Performance = null;
    private ThreadEffectTriggerCondition triggerCondition;

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
        Performance = null;
        TryRefresh();
    }

    public bool Spend()
    {
        if(!available)
            return false;

        available = false;
        image.sprite = cooldownSprite;
        return true;
    }

    public bool Reserve(int duration, Action performance, ThreadEffectTriggerCondition triggerCondition = ThreadEffectTriggerCondition.OnComplete)
    {
        if(!available)
            return false;

        this.Performance = performance;
        remainingCooldown = duration;
        text.text = $"{remainingCooldown}";
        available = false;
        image.sprite = reservedSprite;
        this.triggerCondition = triggerCondition;
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

        if(triggerCondition == ThreadEffectTriggerCondition.EveryTurn && Performance != null)
            Performance();
        
        remainingCooldown--;
        text.text = $"{remainingCooldown}";
        return false;
    }

    private void Refresh()
    {
        available = true;
        image.sprite = availabeSprite;
        text.text = "";
        
        if(Performance != null)
        {
            Performance();
            Performance = null;
        }
    }
}