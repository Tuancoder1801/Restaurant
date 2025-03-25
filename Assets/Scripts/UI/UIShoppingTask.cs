using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShoppingTask : MonoBehaviour
{
    public UIItem uiItem;
    //public GameObject goCashier;
    //public GameObject goHappy;

    public bool followCam;

    void Update()
    {
        if (followCam)
        {
            transform.localEulerAngles = Camera.main.transform.eulerAngles - transform.parent.localEulerAngles;
        }
    }
}
