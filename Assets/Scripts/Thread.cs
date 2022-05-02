using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Thread : MonoBehaviour
{
    [SerializeField] private Image image;
    public bool available {get; private set;}
    public Color activeColor;
    public Color inactiveColor;
    
    public void Toggle(bool state)
    {
        available = state;
        image.color = state ? activeColor : inactiveColor;
    }
}