using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class LongRangeCannonTower : MonoBehaviour, ITower
{
    [SerializeField] private AudioSource audioSource;
    public List<AudioClip> attackClips = new List<AudioClip>();

    [SerializeField] private PlayableDirector director;
    [SerializeField] FollowTargetRotate followTargetRotate;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] Transform firePoint;
    EnemySpawner enemySpawner;
    WaveManager waveManager;
    GameManager gameManager;
    public Index location {get; set;}

    private float orgAttackTime;
    public float attackTime;
    private float nextAttackTime;
    private bool timeToAttack => Time.timeSinceLevelLoad >= nextAttackTime;

    public float minRange;
    private float orgMinRange;

    [field:SerializeField] public float range {get; set;}
    private float orgRange;

    [field:SerializeField] public float damage {get; set;}
    private float orgDamage;
    
    [SerializeField] SpriteRenderer baseRenderer;
    public Sprite disabledBase;
    public Sprite enabledBase;

    [SerializeField] SpriteRenderer turretRenderer;
    public Sprite disabledTurret;
    public Sprite enabledTurret;

    bool attacking = false;

    public Transform target;
    
    private void Awake()
    {
        waveManager = GameObject.FindObjectOfType<WaveManager>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        orgDamage = damage;
        orgRange = range;
        orgAttackTime = attackTime;
        orgMinRange = minRange;
    }

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
        minRange = orgMinRange;
    }

    private void Update()
    {
        if(gameManager.gameOver)
            return;
            
        if(target != null)
        {
            float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
            if(sqrDistanceToTarget < minRange * minRange || sqrDistanceToTarget > range * range)
                target = null;
        }

        if(target == null && !TryAquireTarget())
            return;
        else if(Time.timeSinceLevelLoad >= nextAttackTime && !attacking)
        {
            attacking = true;
            director.Play();
        }
    }

    private bool TryAquireTarget()
    {
        var validEnemies = enemySpawner.enemyList.Where(enemy => enemy != null && (enemy.transform.position - transform.position).sqrMagnitude <= range * range && (enemy.transform.position - transform.position).sqrMagnitude >= minRange * minRange).ToList();
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
        turretRenderer.sprite = disabledTurret;
    }
    public void Enable()
    {
        baseRenderer.sprite = enabledBase;
        turretRenderer.sprite = enabledTurret;
    }

    public void AdjustRange(float percent)
    {
        this.range = range * (1 + percent);
        minRange = minRange * (1 + percent);
    } 
    public void AdjustDamage(float percent) => this.damage = damage * (1 + percent);
    public void AdjustAttackSpeed(float percent) => this.attackTime = attackTime * (1 - percent);

    public void Strike()
    {
        attacking = false;
        if(target == null)
            return;
        nextAttackTime = Time.timeSinceLevelLoad + attackTime;

        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.Fire(target, damage);
        audioSource.PlayOneShot(attackClips.ChooseRandom());
    }
}