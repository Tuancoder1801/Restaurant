using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinRobotItem : MonoBehaviour
{
    public SkinRobotId id;

    public Button bt;
    public Image icon;
    public Image tickIcon;
    public TextMeshProUGUI textPrice;

    public void Load(SkinRobot data)
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
