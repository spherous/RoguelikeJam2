using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Wave 
{
    // later needs to handle multiple types of enemies that could all spawn during a wave
    public List<Group> enemyGroups;
    public float spawnInterval;
    public bool addsThread;
}

[System.Serializable]
public struct Group
{
    public EnemyType type;
    public int count;
}