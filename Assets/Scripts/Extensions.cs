using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T ChooseRandom<T>(this List<T> set) => set[UnityEngine.Random.Range(0, set.Count)];
    public static T ChooseRandom<T>(this T[] set) => set[UnityEngine.Random.Range(0, set.Length)];
}
