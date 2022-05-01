using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static int Floor(this float f) => Mathf.FloorToInt(f);
    public static T ChooseRandom<T>(this List<T> set) => set[UnityEngine.Random.Range(0, set.Count)];
    public static T ChooseRandom<T>(this T[] set) => set[UnityEngine.Random.Range(0, set.Length)];
    public static bool IsEven(this int number) => number % 2 == 0;
    public static bool IsEven(this float number) => number % 2 == 0;
}
