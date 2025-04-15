using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocationBase : MonoBehaviour
{
    public LocationId locationId;
    public Collider box;

    [HideInInspector]
    public GameObject goPlayer;

    private float boxRange = -99;


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

    public float GetBoxRange()
    {
        if (boxRange < 0)
        {
            if (box != null)
            {
                Vector3 size = box.bounds.size;
                float max = size.x;
                if (max < size.y) max = size.y;
                if (max < size.z) max = size.z;

                boxRange = max / 2f + 0.5f;
            }
            else
            {
                boxRange = 0.5f;
            }
        }

        return boxRange;
    }

}
