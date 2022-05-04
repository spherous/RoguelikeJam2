using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private SlicedFilledImage fill;
    [SerializeField] private TextMeshProUGUI text;
    public IHealth health {get; private set;}
    public Gradient gradient;

    public void Track(IHealth health) {
        this.health = health;
        health.onHealthChanged += OnHealthChanged;
        OnHealthChanged(health.currentHP, health.currentHP, health.currentHP / health.maxHP);
    }
    private void OnDestroy() => health.onHealthChanged -= OnHealthChanged;

    private void OnHealthChanged(float oldHP, float newHP, float percent) 
    {
        fill.color = gradient.Evaluate(percent);
        text.text = $"{health.currentHP}";
        fill.fillAmount = percent;  
    }
}