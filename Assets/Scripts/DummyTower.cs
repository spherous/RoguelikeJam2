using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTower : MonoBehaviour
{
    [SerializeField] EnemyList enemyList;
    [SerializeField] FollowTargetRotate followTargetRotate;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyList.enemyList.Count > 0)
        {
            followTargetRotate.targetTransform = enemyList.enemyList[0].transform;
        }
    }
}
