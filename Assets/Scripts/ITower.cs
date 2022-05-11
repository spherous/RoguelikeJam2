using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType {Chain, Web, Range, AOE}

public interface ITower
{
    Index location {get; set;}
    float range {get; set;}
    float damage {get; set;}

    void Disable();
    void Enable();
    void AdjustRange(float percent);
    void AdjustDamage(float percent);
    void AdjustAttackSpeed(float percent);
}