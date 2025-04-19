using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataConstant
{
    public static List<ItemData> items;
    public static List<ItemData> products;
    public static List<MapData> maps;
    public static SkinData skin;

    public static ItemConfig itemConfig;

    public static void Load()
    {
        if (items == null)
        {
            items = Resources.LoadAll<ItemData>("ItemData").ToList();
        }

        if (maps == null)
        {
            maps = Resources.LoadAll<MapData>("MapData").ToList();
        }

        if (skin == null)
        {
            skin = Resources.Load<SkinData>("SkinData/SkinData");
        }

        if (itemConfig == null)
        {
            itemConfig = Resources.Load<ItemConfig>("ItemConfig/ItemConfig");
        }
    }
}
