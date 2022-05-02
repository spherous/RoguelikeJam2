using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IHealth
{
    [SerializeField] private HealthBar healthBar;
    [field:SerializeField] public float maxHP {get; set;}
    public float currentHP {get; set;}
    public int score {get; private set;}

    public event OnHealthChanged onHealthChanged;
    public delegate void OnScoreChanged(int newScore);
    public OnScoreChanged onScoreChanged;

    private void Awake() {
        healthBar.Track(this);
        HealToFull();
    }

    public void HealToFull()
    {
        currentHP = maxHP;
        onHealthChanged?.Invoke(1);
    }

    public void TakeDamage(float damage)
    {
        float startHP = currentHP;
        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        if(currentHP != startHP)
            onHealthChanged?.Invoke(currentHP / maxHP);
        
        if(currentHP == 0)
            Die();
    }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void AddToScore(int amount)
    {
        score += amount;
        onScoreChanged?.Invoke(score);
    }
}