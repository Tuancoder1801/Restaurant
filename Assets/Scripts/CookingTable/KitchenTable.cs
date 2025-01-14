using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenTable : MonoBehaviour
{
    public Tray tray;
    public Machine machine;
    public Plate plate;
    public UILocation uiLocation;
    public Transform uiIndex;

    public int stockTrayNumber;
    public int stockPlateNumber;

    private void Start()
    {
        for (int i = 0; i < tray.itemsPosition.Count; i++)
        {
            tray.itemsPosition[i].maxStackNumber = stockTrayNumber;
        }
        plate.maxStackNumber = stockPlateNumber;

        if (uiLocation != null)
        {
            CreateUILocation();
        }
    }

    private void Update()
    {
        for (int i = 0; i < tray.itemsPosition.Count; i++)
        {
            var item = tray.itemsPosition[i];
            Debug.Log($"Updating UI for ItemId: {item.itemId}, Current: {item.currentStackNumber}, Max: {item.maxStackNumber}");
            uiLocation.SetNumber(item.itemId, item.currentStackNumber, item.maxStackNumber);
        }
    }

    private void CreateUILocation()
    {
        UILocation ui = Instantiate(uiLocation, uiIndex.position, uiIndex.rotation);
        ui.transform.SetParent(this.transform);
        ui.LoadItem(tray.itemsPosition);
    }
}
