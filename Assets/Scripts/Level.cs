using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Level
{
    public List<Wave> waves;   
    public int rows;
    public int cols;
    public int chaos;
}