using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{
    public List<GameObject> enemyList;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(enemyList.Count);
    }
}
