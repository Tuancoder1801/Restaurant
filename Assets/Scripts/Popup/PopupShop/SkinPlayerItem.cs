using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkinPlayerItem : MonoBehaviour
{
    public SkinPlayerId id;

    public Button bt;
    public Image icon;
    public Image tickIcon;
    public TextMeshProUGUI textPrice;

    private Action onClick;

    public void Load(SkinPlayer data)
    {
        id = data.id;
        icon.sprite = data.icon;
        icon.SetNativeSize();
        textPrice.text = data.price.ToString();
    }

    public void SetTick(bool isOn)
    {
        tickIcon.gameObject.SetActive(isOn);
    }

    public void SetPrice(bool isOn)
    {
        textPrice.gameObject.SetActive(isOn);
    }

    public void OnClick(Action action)
    {
        onClick = action;
        bt.onClick.RemoveAllListeners();
        bt.onClick.AddListener(() => onClick?.Invoke());
    }
}
