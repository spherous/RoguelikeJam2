using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AssetLoader
{
    public static ICard[] GetAllCards()
    {
        return Resources.LoadAll("Cards", typeof(ICard)).Select(x => x as ICard).ToArray();
    }
}