using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public Text stackNumberText;
    public ItemId itemId;

    private Tray tray;
    private int currentStackNumber;
    private int maxStackNumber;

    private void Awake()
    {   
        tray = CookingTable.Instance.tray;

        for (int i = 0; i < tray.itemsPosition.Count; i++)
        {
            if (tray.itemsPosition[i].itemId == itemId)
            {
                currentStackNumber = tray.itemsPosition[i].currentStackNumber;
            }
        }


        maxStackNumber = tray.maxStackNumber;
    }

    private void Update()
    {
        for (int i = 0; i < tray.itemsPosition.Count; i++)
        {
            if (tray.itemsPosition[i].itemId == itemId)
            {
                currentStackNumber = tray.itemsPosition[i].currentStackNumber;
            }
        }
        stackNumberText.text = currentStackNumber.ToString() + "/" + maxStackNumber.ToString();
    }
}
