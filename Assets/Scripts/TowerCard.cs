using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class TowerCard : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite artwork;


}
