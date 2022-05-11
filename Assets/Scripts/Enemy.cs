using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IHealth
{
    EnemySpawner enemySpawner;
    ProcGen procGen;
    GameManager gameManager;

    private int pathingToNode = 1;
    [SerializeField] Rigidbody2D rb;
    [field:SerializeField] public float speed {get; set;}

    public event OnHealthChanged onHealthChanged;
    [field:SerializeField] public float maxHP {get; set;}
    public float currentHP {get; set;}

    public int scoreValue;

    protected void Start()
    {
        enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
        procGen = GameObject.FindObjectOfType<ProcGen>();
        gameManager = GameObject.FindObjectOfType<GameManager>();

        HealToFull();
    }

    private void OnDestroy() => enemySpawner.Remove(this);

    protected void FixedUpdate()
    {
        Vector3 vec = procGen.path[pathingToNode].transform.position - transform.position;
        Vector3 dir = vec.normalized;
        transform.up = dir;
        rb.velocity = dir * speed;
        
        if(vec.magnitude < 0.05f)
            pathingToNode++;

        if(pathingToNode > procGen.path.Count - 1)
        {
            gameManager.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        float startHP = currentHP;
        currentHP = currentHP - damage >= 0 ? currentHP - damage : 0;

        if(currentHP != startHP)
            onHealthChanged?.Invoke(this, startHP, currentHP, currentHP / maxHP);
        
        if(currentHP == 0)
            Die();
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

    public void Die()
    {
        gameManager.AddToScore(scoreValue);
        Destroy(gameObject);
    }

    public void AdjustSpeed(float amount) => speed = Mathf.Clamp(amount + speed, 0, speed + Mathf.Abs(amount));
    
    public void AdjustHealth(float percent) 
    {
        float startHP = currentHP;
        maxHP *= 1 - percent;
        currentHP *= 1 - percent;
        onHealthChanged?.Invoke(this, startHP, currentHP, currentHP / maxHP);
    }
}