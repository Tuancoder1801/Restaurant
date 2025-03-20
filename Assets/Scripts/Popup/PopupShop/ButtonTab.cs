using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonTab : MonoBehaviour
{
    public Button tab;

    public void OnClick(UnityAction action)
    {
        tab.onClick.AddListener(action);
    }
}
