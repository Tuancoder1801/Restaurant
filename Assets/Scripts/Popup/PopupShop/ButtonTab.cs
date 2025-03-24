using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonTab : MonoBehaviour
{
    public SkinTagType type;
    public Button tab;

    private void Start()
    {
        tab.onClick.AddListener(OnClicḳ); 
    }

    private void OnClicḳ()
    {
        FindObjectOfType<PopupShop>().ShowView(type);
    }
}
