using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class BasePopup : MonoBehaviour
{
    public Button btExit;

    protected virtual void Start()
    {
        btExit.onClick.AddListener(ClickExit);
    }

    protected void ClickExit()
    {
        AudioManager.Instance.TurnOnButtonSound();

        gameObject.SetActive(false);
        UIGame.Instance.ShowButtons();
        ShopAreaController.Instance.Hide();
    }
}
