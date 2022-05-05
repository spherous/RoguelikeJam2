using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, IHealth
{

    public float speed;

    EnemyList enemyList;
    ProcGen procGen;
    GameManager gameManager;
    [SerializeField] Rigidbody2D rb;

    private float lifeTime;
    public float deathRate;

    private int pathingToNode = 1;

    public event OnHealthChanged onHealthChanged;

    [field:SerializeField] public float maxHP {get; set;}
    public float currentHP {get; set;}

    void Start()
    {
        lifeTime = Time.timeSinceLevelLoad + deathRate;
        enemyList = GameObject.FindObjectOfType<EnemyList>();
        procGen = GameObject.FindObjectOfType<ProcGen>();
        gameManager = GameObject.FindObjectOfType<GameManager>();

        HealToFull();
    }

    private void OnDestroy() {
        enemyList.enemyList.Remove(gameObject);
    }

    void FixedUpdate()
    {
        Vector3 vec = (procGen.path[pathingToNode].transform.position - transform.position);
        Vector3 dir = vec.normalized;
        transform.up = dir;
        rb.velocity = dir * speed;
        
        if(vec.magnitude < 0.01f)
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
            onHealthChanged?.Invoke(startHP, currentHP, currentHP / maxHP);
        
        if(currentHP == 0)
            Die();
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

    public void Die()
    {
        gameManager.AddToScore(1);
        Destroy(gameObject);
    }
}