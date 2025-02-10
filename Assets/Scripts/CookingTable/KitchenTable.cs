using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenTable : MonoBehaviour
{   
    public TableId tableId;
    public ItemId itemId;
    public Tray tray;
    public Machine machine;
    public Plate plate;
    public UILocation uiLocation;

    public int stockTrayNumber;
    public int stockPlateNumber;

    public Transform chefIndex;
    public Transform porterIndex;
    public Transform waiterIndex;

    private void Start()
    {   
        for (int i = 0; i < tray.itemsPosition.Count; i++)
        {
            tray.itemsPosition[i].maxStackNumber = stockTrayNumber;
        }
        plate.maxStackNumber = stockPlateNumber;

        uiLocation.LoadItem(tray.itemsPosition);
    }

    private void Update()
    {
        for (int i = 0; i < tray.itemsPosition.Count; i++)
        {
            var item = tray.itemsPosition[i];
            uiLocation.SetNumber(item.itemId, item.currentStackNumber, stockTrayNumber);
        }
    }
}
