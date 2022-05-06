using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static int Floor(this float f) => Mathf.FloorToInt(f);
    public static int Mod(this int a, int b) => (a % b + b) % b;
    public static float Mod(this float a, float b) => (a % b + b) % b;
    public static bool IsEven(this int number) => number % 2 == 0;
    public static bool IsEven(this float number) => number % 2 == 0;
    public static List<T> Shuffle<T>(this List<T> set)
    {
        int n = set.Count;
        while(n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = set[k];
            set[k] = set[n];
            set[n] = value;
        }
        return set;
    }

    public static string GetStringFromSeconds(this float seconds) => seconds < 60 
        ? @"%s\.f" 
        : seconds < 3600
            ? @"%m\:%s\.f"
            : @"%h\:%m\:%s\.f";

    public static T ChooseRandom<T>(this List<T> set) => set[UnityEngine.Random.Range(0, set.Count)];
    public static T ChooseRandom<T>(this T[] set) => set[UnityEngine.Random.Range(0, set.Length)];
}
