using System;
using UnityEngine;

public enum DebuffType {None = 0, Slow = 1, Speed = 2, LessHP = 3, StutterStep = 4}

public static class DebuffTypeExtensions
{
    public static Action GetEffect(this DebuffType type) => type switch{
        DebuffType.Slow => () => AdjustSpeed(-0.333f),
        DebuffType.Speed => () => AdjustSpeed(0.333f),
        DebuffType.LessHP => () => AdjustHealth(0.25f),
        DebuffType.StutterStep => () => {},
        _ => null
    };

    public static void AdjustSpeed(float amount)
    {
        WaveManager waveManager = GameObject.FindObjectOfType<WaveManager>();
        waveManager.AdjustSpeed(amount);
    }
    public static void AdjustHealth(float percent)
    {
        WaveManager waveManager = GameObject.FindObjectOfType<WaveManager>();
        waveManager.AdjustHealth(percent);
    }
}