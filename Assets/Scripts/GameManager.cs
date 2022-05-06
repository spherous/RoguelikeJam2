using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IHealth
{
    [SerializeField] private SceneTransition sceneTransitionPrefab;
    [SerializeField] private Transform screen;
    [SerializeField] private ThreadPool threadPoolPrefab;
    private ThreadPool threadPool;

    [SerializeField] private HealthBar healthBarPrefab;
    private HealthBar healthBar;
    [field:SerializeField] public float maxHP {get; set;}
    public float currentHP {get; set;}
    public int score {get; private set;}

    public event OnHealthChanged onHealthChanged;
    public delegate void OnScoreChanged(int newScore);
    public OnScoreChanged onScoreChanged;

    private void Awake()
    {
        threadPool = Instantiate(threadPoolPrefab, screen);
        threadPool.IncreaseThreadCount(3);

        healthBar = Instantiate(healthBarPrefab, screen);
        healthBar.Track(this);

        HealToFull();
    }

    public void Heal(float amount)
    {
        float startHP = currentHP;
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);

        if(currentHP != startHP)
            onHealthChanged?.Invoke(startHP, currentHP, currentHP / maxHP);
    }

    public void HealToFull()
    {
        float oldHP = currentHP;
        currentHP = maxHP;
        onHealthChanged?.Invoke(oldHP, currentHP, 1);
    }

    public void TakeDamage(float damage)
    {
        float startHP = currentHP;
        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        if(currentHP != startHP)
            onHealthChanged?.Invoke(startHP, currentHP, currentHP / maxHP);
        
        if(currentHP == 0)
            Die();
    }

    public void Die()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneTransition transition = FindObjectOfType<SceneTransition>();
        if(transition)
            transition.Transition(currentSceneName);
        else if(sceneTransitionPrefab != null)
        {
            transition = Instantiate(sceneTransitionPrefab, screen);
            transition.Transition(currentSceneName);
        }
        else
            SceneManager.LoadScene(currentSceneName, LoadSceneMode.Single);
    }

    public void AddToScore(int amount)
    {
        score += amount;
        onScoreChanged?.Invoke(score);
    }
}