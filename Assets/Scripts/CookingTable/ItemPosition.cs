using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPosition : MonoBehaviour
{
    public ItemId itemId;
    public List<Transform> itemPositions;
    public int currentStackNumber = 0;
    public List<BaseItem> items;

    private bool popLock; 

    public void Init()
    {
        if(items == null) items = new List<BaseItem>();

        if(items.Count < itemPositions.Count)
        {
            for(int i = items.Count; i < itemPositions.Count; i++)
            {
                items.Add(null);
            }
        }

        popLock = false;
    }

    public void PushItem(BaseItem item, int index)
    {
        items[index] = item;
    }

    public BaseItem PopItem()
    {
        if(popLock) return null;
        popLock = true;
        int index = items.FindLastIndex(x => x != null);

        if(index >= 0)
        {
            var item = items[index];
            items[index] = null;

            SortItem();

            popLock = false;
            return item;
        }
        popLock = false;
        return null;
    }

    public bool IsHasItem()
    {
        return items.Any(x => x != null);
    }

    public int GetIndexEmpty()
    {
        return items.FindIndex(x => x == null);
    }

    public bool IsFullStack()
    {
        return CountItem() >= currentStackNumber;
    }

    public bool IsEmpty()
    {
        return CountItem() <= 0;
    }

    public int CountItem()
    {
        return items.Count(x => x != null);
    }

    private void SortItem()
    {
        int indexNull = -1;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                if (indexNull < 0) indexNull = i;
            }
            else
            {
                if (indexNull >= 0)
                {
                    // change position
                    Vector3 vTemp = itemPositions[i].localPosition;
                    itemPositions[i].localPosition = itemPositions[indexNull].localPosition;
                    itemPositions[indexNull].localPosition = vTemp;

                    var temp = itemPositions[i];
                    itemPositions[i] = itemPositions[indexNull];
                    itemPositions[indexNull] = temp;

                    items[indexNull] = items[i];
                    items[i] = null;

                    // find new null
                    indexNull = i;
                    for (int j = 0; j <= i; j++)
                    {
                        if (items[j] == null)
                        {
                            indexNull = j;
                            break;
                        }
                    }
                }
            }
        }
    }
}

