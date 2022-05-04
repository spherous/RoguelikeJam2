using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTower : MonoBehaviour, ITower
{
    EnemyList enemyList;
    [SerializeField] FollowTargetRotate followTargetRotate;
    void Start()
    {
        enemyList = GameObject.FindObjectOfType<EnemyList>();
    }

    void Update()
    {
        if(enemyList.enemyList.Count > 0)
            followTargetRotate.targetTransform = enemyList.enemyList[0].transform;
    }
}
