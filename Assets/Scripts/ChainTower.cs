using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainTower : MonoBehaviour, ITower
{
    EnemyList enemyList;
    [SerializeField] FollowTargetRotate followTargetRotate;
    public float attackTime;
    private float nextAttackTime;
    private bool timeToAttack => Time.timeSinceLevelLoad >= nextAttackTime;
    public Transform target;
    public float damage;
    public int chainCount;

    [SerializeField] private Projectile projectilePrefab;

    void Start()
    {
        enemyList = GameObject.FindObjectOfType<EnemyList>();
    }

    void Update()
    {
        if(target == null && !TryAquireTarget())
            return;

        if(timeToAttack)
            Attack();
    }

    private void Attack()
    {
        nextAttackTime = Time.timeSinceLevelLoad + attackTime;
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Fire(target, damage, chainCount);
    }

    private bool TryAquireTarget()
    {
        if(enemyList.enemyList.Count > 0)
        {
            target = ((MonoBehaviour)enemyList.enemyList[0]).transform;
            followTargetRotate.targetTransform = target;
            return true;
        }
        return false;
    }
}