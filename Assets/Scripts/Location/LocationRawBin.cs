using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationRawBin : LocationBase
{
    public BaseItem item;
    public Transform posStart;

    public override BaseItem PopItem()
    {
        if(item.itemId == ItemId.None) return null;

        var i = Instantiate(item, posStart.position, posStart.rotation);
        return i;
    }

    public override bool HasProductItem()
    {
        return true;
    }

    public override ItemId GetProductId()
    {
        return item.itemId;
    }
}
