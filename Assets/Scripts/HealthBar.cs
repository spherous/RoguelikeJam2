using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

public class HealthBar : SerializedMonoBehaviour
{
    [SerializeField] private SlicedFilledImage fill;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private SlicedFilledImage bg;
    [SerializeField] private GroupFader fader;
    bool inDeath = false;
    public float fps;
    private int passedFrames = 0;
    private int childIndex = 0;
    private int parentIndex = 0;
    [OdinSerialize] public List<List<Sprite>> sprites = new List<List<Sprite>>();
    public List<Sprite> deathLoop = new List<Sprite>();
    public IHealth health {get; private set;}
    public Gradient gradient;

    public void Track(IHealth health)
    {
        this.health = health;
        health.onHealthChanged += OnHealthChanged;
        OnHealthChanged(health, health.currentHP, health.currentHP, health.currentHP / health.maxHP);
    }
    private void OnDestroy() => health.onHealthChanged -= OnHealthChanged;

    private void Update()
    {
        if(passedFrames > 60f / (float)fps)
        {
            passedFrames = 0;
            if(inDeath)
            {
                childIndex++;
                if(childIndex > deathLoop.Count - 1)
                {
                    inDeath = false;
                    Hide();
                    return;
                }
                bg.sprite = deathLoop[childIndex];
            }
            else if(health.currentHP != 0)
            {
                childIndex = childIndex == sprites[parentIndex].Count - 1 ? 0 : childIndex + 1;
                bg.sprite = sprites[parentIndex][childIndex];
            }
        }
        else
            passedFrames++;
    }

    public void Hide() => fader.FadeOut();

    private void OnHealthChanged(IHealth changed, float oldHP, float newHP, float percent) 
    {
        if(newHP == 0)
        {
            inDeath = true;
            childIndex = 0;
        }
        else
        {
            int newParent = (7f - (7f * percent)).Floor();
            if(newParent != parentIndex)
            {
                parentIndex = newParent;
                childIndex = 0;
            }
        }

        fill.color = gradient.Evaluate(percent);
        text.text = $"{health.currentHP}";
        fill.fillAmount = percent;  
    }
}