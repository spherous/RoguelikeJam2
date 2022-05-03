using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Wave 
{
    // later needs to handle multiple types of enemies that could all spawn during a wave
    public int enemyCount;
    public float spawnInterval;
}