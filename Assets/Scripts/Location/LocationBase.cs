using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationBase : MonoBehaviour
{
    public LocationId locationId;

    [HideInInspector]
    public GameObject goPlayer;

    public virtual Vector3 GetPosProduct()
    {
        return transform.position;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StaticValue.CHARACTER_NAME_TAG))
        {
            goPlayer = other.gameObject;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StaticValue.CHARACTER_NAME_TAG))
        {
            goPlayer = null;
        }
    }

    public virtual BaseItem PopItem() 
    {
        return null;
    }

    public virtual void PushItem(BaseItem item)
    {

    }

    public virtual int MaxProductCanMake()
    {
        return 0;
    }

    public virtual bool HasProductItem()
    {
        return false;
    }

    public virtual List<ItemId> GetNeedItems()
    {
        return null;
    }

    public virtual ItemId GetProductId() 
    {
        return ItemId.None;
    }

    public virtual Vector3 GetPosRawBin()
    {
        return transform.position;
    }

    public virtual List<double> GetMoneys()
    {
        return null;
    }

    public virtual void LoadMoneys(List<double> moneys)
    {

    }

    public virtual void CustomerSit()
    {
    }
}
