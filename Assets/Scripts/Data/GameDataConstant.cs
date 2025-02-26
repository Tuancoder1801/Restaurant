using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataConstant
{
    public static List<ItemData> items;
    public static List<ItemData> products;
    public static List<MapData> maps;

    public static void Load()
    {
        if (items == null)
        {
            items = Resources.LoadAll<ItemData>("ItemData").ToList();
        }

        if (products == null)
        {
            products = Resources.LoadAll<ItemData>("ProductData").ToList();
        }

        if (maps == null)
        {
            maps = Resources.LoadAll<MapData>("MapData").ToList();
        }
    }
}
