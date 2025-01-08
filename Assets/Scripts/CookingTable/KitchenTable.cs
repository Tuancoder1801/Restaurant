using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenTable : MonoBehaviour
{
    public Tray tray;
    public Machine machine;
    public Plate plate;

    public int stockTrayNumber;
    public int stockPlateNumber;

    public UILocation uiLocation;

    private void Awake()
    {
        tray.maxStackNumber = stockTrayNumber;
        plate.maxStackNumber = stockPlateNumber;
    }
    

}
