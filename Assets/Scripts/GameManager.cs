using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IHealth
{
    [SerializeField] private SceneTransition sceneTransitionPrefab;
    [SerializeField] private Transform screen;
    [SerializeField] private ThreadPool threadPoolPrefab;
    [SerializeField] private ProcGen procGen;
    private ThreadPool threadPool;

    [SerializeField] private HealthBar healthBarPrefab;
    private HealthBar healthBar;
    [field:SerializeField] public float maxHP {get; set;}
    public float currentHP {get; set;}
    public int score {get; private set;}

    public event OnHealthChanged onHealthChanged;
    public delegate void OnScoreChanged(int newScore);
    public OnScoreChanged onScoreChanged;

    public bool gameOver {get; private set;} = false;

    private void Awake()
    {
        threadPool = Instantiate(threadPoolPrefab, screen);
        threadPool.IncreaseThreadCount(3);

        healthBar = Instantiate(healthBarPrefab, screen);
        HealToFull();
        healthBar.Track(this);
    }

    public void Heal(float amount)
    {
        float startHP = currentHP;
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);

        if(currentHP != startHP)
            onHealthChanged?.Invoke(this, startHP, currentHP, currentHP / maxHP);
    }

    public void HealToFull()
    {
        float oldHP = currentHP;
        currentHP = maxHP;
        onHealthChanged?.Invoke(this, oldHP, currentHP, 1);
    }

    public void TakeDamage(float damage)
    {
        float startHP = currentHP;
        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        if(currentHP != startHP)
            onHealthChanged?.Invoke(this, startHP, currentHP, currentHP / maxHP);
        
        if(currentHP == 0)
            Die();
    }

    public void AdjustHealth(float percent) 
    {
        float startHP = currentHP;
        maxHP *= 1 - percent;
        currentHP *= 1 - percent;
        onHealthChanged?.Invoke(this, startHP, currentHP, currentHP / maxHP);
    } 

    public void Die()
    {
        gameOver = true;
        // procGen.kingdom.GameOver();
        // string currentSceneName = SceneManager.GetActiveScene().name;
        // SceneTransition transition = FindObjectOfType<SceneTransition>();
        // if(transition)
        //     transition.Transition(currentSceneName);
        // else if(sceneTransitionPrefab != null)
        // {
        //     transition = Instantiate(sceneTransitionPrefab, screen);
        //     transition.Transition(currentSceneName);
        // }
        // else
        //     SceneManager.LoadScene(currentSceneName, LoadSceneMode.Single);
    }

    public void AddToScore(int amount)
    {
        score += amount;
        onScoreChanged?.Invoke(score);
    }
}