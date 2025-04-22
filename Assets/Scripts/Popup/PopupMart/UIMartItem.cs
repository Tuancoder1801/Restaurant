using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMartItem : MonoBehaviour
{
    public TextMeshProUGUI txtNameFood;
    public TextMeshProUGUI txtLocation;
    public TextMeshProUGUI txtUnlock;

    public Button btnGoTo;

    public void Load(MapData data)
    {
        txtNameFood.text = data.nameRestaurant;

    }
}
