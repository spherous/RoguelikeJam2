using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void OnHealthChanged(float oldHP, float newHP, float percent);

public interface IHealth 
{
    float maxHP {get; set;}   
    float currentHP {get; set;}
    void TakeDamage(float damage);
    void HealToFull();
    void Die();
    event OnHealthChanged onHealthChanged;
}