using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTower : MonoBehaviour
{
    EnemyList enemyList;
    [SerializeField] FollowTargetRotate followTargetRotate;
    void Start()
    {
        enemyList = GameObject.Find("GameManager").GetComponent<EnemyList>();
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
