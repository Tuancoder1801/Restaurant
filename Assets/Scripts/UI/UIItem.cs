using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public ItemId item;
    public Image imgIcon;
    public Text txtNumber;

    public void LoadItem(ItemId itemId)
    {
        item = itemId; 
        
        for(int i = 0; i < GameDataConstant.items.Count; i++)
        {
            if (itemId == GameDataConstant.items[i].itemId)
            {
                imgIcon.sprite = GameDataConstant.items[i].icon;
                imgIcon.SetNativeSize();
            }
        }
    }

    public void SetNumber(int num, int max)
    {
        txtNumber.text = num + "/" + max;
    }
}
