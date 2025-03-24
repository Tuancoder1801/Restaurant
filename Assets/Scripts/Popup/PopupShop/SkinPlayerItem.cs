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

    public void Load(SkinPlayer data)
    {
        id = data.id;
        icon.sprite = data.icon;
        icon.SetNativeSize();
        textPrice.text = data.price.ToString();
    }

    private void Start()
    {
        bt.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        //FindObjectOfType<ViewSkinPlayer>().Select(id);
    }
}
