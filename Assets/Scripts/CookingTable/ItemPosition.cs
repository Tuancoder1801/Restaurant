using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPosition : MonoBehaviour
{
    public ItemId itemId;
    public List<Transform> itemPositions;
    public int currentStackNumber = 0;
    public int maxStackNumber = 4;

    public bool isHasItem()
    {
        return itemPositions.Any(pos => pos.GetComponent<BaseItem>() != null);
    }

    public BaseItem GetItem()
    {
        if (itemPositions != null) return null;

        for (int i = 0; i < itemPositions.Count; i++)
        {
            BaseItem item = itemPositions[i].GetComponent<BaseItem>();

            return item;
        }

        return null;
    }
}

