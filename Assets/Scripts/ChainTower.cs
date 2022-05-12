using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainTower : MonoBehaviour, ITower
{
    EnemySpawner enemySpawner;
    [SerializeField] FollowTargetRotate followTargetRotate;
    [SerializeField] Transform firePoint;
    [SerializeField] SpriteRenderer fisty;
    WaveManager waveManager;
    private float orgAttackTime;
    public float attackTime;
    private float nextAttackTime;
    private bool timeToAttack => Time.timeSinceLevelLoad >= nextAttackTime;
    public Transform target;
    public int chainCount;

    [SerializeField] SpriteRenderer baseRenderer;
    public Sprite disabledBase;
    public Sprite enabledBase;

    [SerializeField] SpriteRenderer chainRenderer;
    public Sprite disabledChain;
    public Sprite enabledChain;

    [SerializeField] SpriteRenderer fistyRenderer;
    public Sprite disabledFisty;
    public Sprite enabledFisty;

    [SerializeField] SpriteRenderer rotationPointRenderer;
    public Sprite diabledRotatePoint;
    public Sprite enabledRotatePoint;

    [SerializeField] private Projectile projectilePrefab;
    float fistyReloadTime;

    [field:SerializeField] public float range {get; set;}
    private float orgRange;

    [field:SerializeField] public float damage {get; set;}
    private float orgDamage;
    public Index location {get; set;}

    private void Awake()
    {
        waveManager = GameObject.FindObjectOfType<WaveManager>();
        orgDamage = damage;
        orgRange = range;
        orgAttackTime = attackTime;
    }

    void Start()
    {
        fisty.enabled = true;
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

    void Update()
    {
        if(Time.timeSinceLevelLoad >= fistyReloadTime && !fisty.enabled)
            fisty.enabled = true;

        if(target == null && !TryAquireTarget())
            return;

        if(timeToAttack)
            Attack();
    }

    private void Attack()
    {
        nextAttackTime = Time.timeSinceLevelLoad + attackTime;
        fistyReloadTime = Time.timeSinceLevelLoad + attackTime / 2;
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.Fire(target, damage, chainCount);
        fisty.enabled = false;
    }

    private bool TryAquireTarget()
    {
        var validEnemies = enemySpawner.enemyList.Where(enemy => enemy != null && (enemy.transform.position - transform.position).sqrMagnitude <= range * range).ToList();
        if(validEnemies.Count > 0)
        {
            target = validEnemies[0].transform;
            followTargetRotate.targetTransform = target;
            return true;
        }
        return false;
    }

    public void Disable()
    {
        baseRenderer.sprite = disabledBase;
        chainRenderer.sprite = disabledChain;
        fistyRenderer.sprite = disabledFisty;
        rotationPointRenderer.sprite = diabledRotatePoint;
    }
    public void Enable()
    {
        baseRenderer.sprite = enabledBase;
        chainRenderer.sprite = enabledChain;
        fistyRenderer.sprite = enabledFisty;
        rotationPointRenderer.sprite = enabledRotatePoint;
    }

    public void AdjustRange(float percent) => this.range = range * (1 + percent);
    public void AdjustDamage(float percent) => this.damage = damage * (1 + percent);
    public void AdjustAttackSpeed(float percent) => this.attackTime = attackTime * (1 - percent);
}