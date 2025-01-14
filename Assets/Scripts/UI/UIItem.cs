using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public ItemId itemId;
    public Image imgIcon;
    public Text txtNumber;

    public void LoadItem(ItemId itemId)
    {
        this.itemId = itemId; 
        
        for(int i = 0; i < GameDataConstant.items.Count; i++)
        {
            if (itemId == GameDataConstant.items[i].itemId)
            {
                imgIcon.sprite = GameDataConstant.items[i].icon;
                imgIcon.SetNativeSize();
            }
        }
    }

    public void SetNumber(ItemId itemId,int num, int max)
    {
        if (this.itemId == itemId)
        {
            txtNumber.text = num + "/" + max;
        }
    }
}
