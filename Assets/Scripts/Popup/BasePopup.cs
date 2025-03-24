using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class BasePopup : MonoBehaviour
{
    public Button btExit;

    protected void Awake()
    {
        btExit.onClick.AddListener(ClickExit);
    }

    protected void ClickExit()
    {
        gameObject.SetActive(false);
    }
}
