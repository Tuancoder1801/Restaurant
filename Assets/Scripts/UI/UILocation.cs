using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILocation : MonoBehaviour
{   
    public UIItem uiItem;
    public Transform itemContent;
    public Tray tray;

    private void Start()
    {
        transform.localEulerAngles = Camera.main.transform.eulerAngles - transform.parent.localEulerAngles;
        CreateUIItem();
    }

    private void Update()
    {
        UpdateUIItemNumbers();
    }

    private void CreateUIItem()
    {
        for(int i = 0; i < tray.itemsPosition.Count; i++)
        {
            UIItem ui = Instantiate(uiItem);
            ui.transform.SetParent(itemContent);
            ui.transform.localScale = Vector3.one;
            ui.transform.localEulerAngles = Vector3.zero;
            ui.transform.localPosition = Vector3.zero;
            ui.LoadItem(tray.itemsPosition[i].itemId);
            ui.SetNumber(tray.itemsPosition[i].currentStackNumber, tray.maxStackNumber);
        }
    }

    private void UpdateUIItemNumbers()
    {
        for (int i = 0; i < tray.itemsPosition.Count; i++)
        { 
            if (i < itemContent.childCount)
            {
                UIItem uiItem = itemContent.GetChild(i).GetComponent<UIItem>();
                uiItem.SetNumber(tray.itemsPosition[i].currentStackNumber, tray.maxStackNumber);
            }
        }
    }
}
