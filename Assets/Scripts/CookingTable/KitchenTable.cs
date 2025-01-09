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

    private void Awake()
    {
        tray.maxStackNumber = stockTrayNumber;
        plate.maxStackNumber = stockPlateNumber;

        if(uiLocation != null)
        {
            CreateUILocation();
        }
    }

    private void CreateUILocation()
    {
        UILocation ui = Instantiate(uiLocation, uiIndex.position, uiIndex.rotation);
        ui.transform.SetParent(this.transform);
        ui.tray = tray;
    }
}
