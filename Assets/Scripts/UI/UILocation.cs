using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UILocation : MonoBehaviour
{
    public GameObject uiItem;
    public GameObject itemContent;
    
    public List<UIItem> uiItems;

    private void Start()
    {
        transform.localEulerAngles = Camera.main.transform.eulerAngles - transform.parent.localEulerAngles;
    }

    public void LoadItem(List<ItemPosition> materials)
    {
        if (uiItems == null) uiItems = new List<UIItem>();
        else uiItems.ForEach(x => x.gameObject.SetActive(false));

        for(int i = 0; i < materials.Count; i++)
        {
            UIItem uiItem = null;
            if ( i < uiItems.Count)
            {
                uiItem = uiItems[i];
            }
            else
            {
                uiItem = CreateUIItem();
                uiItems.Add(uiItem);
            }

            uiItem.gameObject.SetActive(true);
            uiItem.LoadItem(materials[i].itemId);
            uiItem.SetNumber(materials[i].CountItem(), materials[i].currentStackNumber);
        }

        itemContent.SetActive(true);
    }

    public void LoadProduct(List<ItemOrder> itemOrders)
    {
        if (uiItems == null) uiItems = new List<UIItem>();
        else uiItems.ForEach(x => x.gameObject.SetActive(false));

        for (int i = 0; i < itemOrders.Count; i++)
        {
            UIItem uiItem = null;
            if (i < uiItems.Count)
            {
                uiItem = uiItems[i];
            }
            else
            {
                uiItem = CreateUIItem();
                uiItems.Add(uiItem);
            }

            uiItem.gameObject.SetActive(true);
            uiItem.LoadProduct(itemOrders[i].itemId);
            uiItem.SetNumber(itemOrders[i].currentItemNumber, itemOrders[i].quantity);
        }

        itemContent.SetActive(true);
    }

    public bool HasItem(ItemId itemType)
    {
        if (uiItems != null)
        {
            return uiItems.FirstOrDefault(x => x.gameObject.activeSelf && x.item == itemType);
        }
        return false;
    }

    private UIItem CreateUIItem()
    {
        GameObject ui = Instantiate(uiItem, Vector3.zero, uiItem.transform.rotation);
        ui.transform.SetParent(itemContent.transform);
        ui.transform.localScale = Vector3.one;
        ui.transform.localEulerAngles = Vector3.zero;
        ui.transform.localPosition = Vector3.zero;

        return ui.GetComponent<UIItem>();
    }
    
    public void SetNumber(ItemId itemId, int num, int max)
    {
        var ui = uiItems.FirstOrDefault(x => x.item == itemId);
        if (ui != null)
        {
            ui.SetNumber(num, max);
        }
        else
        {
            Debug.LogWarning($"UIItem with ItemId {itemId} not found.");
        }
    }

    public void HideUIItem(ItemId itemId)
    {
        var ui = uiItems.FirstOrDefault(x => x.item == itemId);

        if(ui != null) ui.gameObject.SetActive(false);

        if(uiItems.All(x => !x.gameObject.activeSelf)) itemContent.SetActive(false);
    }
}
