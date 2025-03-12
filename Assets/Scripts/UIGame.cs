using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    public TextMeshProUGUI textMoney;

    private float currentMoney;
    private bool isAnimatingMoney = false;

    private void Awake()
    {
        currentMoney = 0;
        textMoney.text = currentMoney.ToString(); 
    }

    public void AddMoney()
    {

    }
}
