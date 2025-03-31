using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemConfig", menuName = "ScriptableObjects/ItemConfig")]

public class ItemConfig : ScriptableObject
{
    public List<ItemData> items;
    public List<double> rateItemBuys;
    public int sMax;
    public int sMin;

    public int sVipMax;
    public int sVipMin;

    public int GetItemCountBuy(int max)
    {
        if (max <= 1) return 1;

        int count = 1;
        max = max - 1;

        System.Random rnd = new System.Random();
        for (int i = 0; i < rateItemBuys.Count; i++)
        {
            double rate = rnd.NextDouble() * 100;
            if (i < max && rate < rateItemBuys[i])
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    public int GetItemPrice(ItemId itemId)
    {
        var item = items.FirstOrDefault(x => x.itemId == itemId);
        if (item != null) return item.price;
        return 0;
    }
}
