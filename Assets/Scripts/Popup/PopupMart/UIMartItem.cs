using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMartItem : MonoBehaviour
{
    public int mapId;
    public TextMeshProUGUI txtNameFood;
    public TextMeshProUGUI txtLocation;
    public TextMeshProUGUI txtlock;

    public Button btnGoTo;

    public void Load(MapData data)
    {   
        mapId = data.MapId;
        txtNameFood.text = data.nameRestaurant;

        txtLocation.gameObject.SetActive(false);
        txtlock.gameObject.SetActive(false);
        btnGoTo.gameObject.SetActive(false);
    }
}
