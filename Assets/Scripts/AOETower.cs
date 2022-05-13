using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class AOETower : MonoBehaviour, ITower
{
    [SerializeField] private PlayableDirector director;
    EnemySpawner enemySpawner;
    WaveManager waveManager;
    public Index location {get; set;}

    private float orgAttackTime;
    public float attackTime;
    private float nextAttackTime;
    private bool timeToAttack => Time.timeSinceLevelLoad >= nextAttackTime;

    [field:SerializeField] public float range {get; set;}
    private float orgRange;

    [field:SerializeField] public float damage {get; set;}
    private float orgDamage;
    
    [SerializeField] SpriteRenderer baseRenderer;
    public Sprite disabledBase;
    public Sprite enabledBase;

    bool attacking = false;

    void Start()
    {
        enemySpawner = GameObject.FindObjectOfType<EnemySpawner>();
        if(waveManager != null)
            waveManager.onWaveComplete += OnWaveComplete;
    }

    private void OnDestroy()
    {
        if(waveManager != null)
            waveManager.onWaveComplete -= OnWaveComplete;    
    }

    private void OnWaveComplete(Wave wave)
    {
        damage = orgDamage;
        range = orgRange;
        attackTime = orgAttackTime;
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad >= nextAttackTime && !attacking && enemySpawner.CheckIfEnemiesWithinDistanceOfLocation(transform.position, range))
        {
            attacking = true;
            director.Play();
        }
    }

    public void Disable()
    {
        baseRenderer.sprite = disabledBase;
    }
    public void Enable()
    {
        baseRenderer.sprite = enabledBase;
    }

    public void AdjustRange(float percent) => this.range = range * (1 + percent);
    public void AdjustDamage(float percent) => this.damage = damage * (1 + percent);
    public void AdjustAttackSpeed(float percent) => this.attackTime = attackTime * (1 - percent);

    public void Strike()
    {
        attacking = false;
        nextAttackTime = Time.timeSinceLevelLoad + attackTime;

        RaycastHit2D[] hitEnemies = Physics2D.CircleCastAll(transform.position, range, Vector2.zero, LayerMask.NameToLayer("Enemy"));
        foreach(RaycastHit2D hit in hitEnemies)
        {
            if(hit.collider.TryGetComponent(out Enemy enemy))
                enemy.TakeDamage(damage);
        }
    }
}