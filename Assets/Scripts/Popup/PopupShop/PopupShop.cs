using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public enum SkinTagType
{
    Player,
    Glasses,
    Robot
}

public class PopupShop : BasePopup
{
    public GameObject[] views;
    public ButtonTab[] tabs;

    private SkinTagType currentTab = SkinTagType.Player;

    private void OnEnable()
    {
        ShowCurrentView();
    }

    public void ShowView(SkinTagType tab)
    {
        if (currentTab != tab)
        {
            currentTab = tab;
            ShowCurrentView();
        }
    }

    private void ShowCurrentView()
    {
        for (int i = 0; i < views.Length; i++)
        {
            views[i].SetActive(i == (int)currentTab);
        }
    }
}
