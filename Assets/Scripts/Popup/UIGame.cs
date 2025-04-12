using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIGame : Singleton<UIGame>
{   
    [Header("Money")]
    public TextMeshProUGUI textMoney;

    [Header("Popups")]
    public Button btShop; 
    public PopupShop shop;
    
    public double currentMoney = 0;
    private Coroutine moneyCoroutine;

    private void Awake()
    {
        UpdateMoneyText(currentMoney);
        btShop.onClick.AddListener(ClickButtonShop);
    }

    #region money

    public void AddMoney(double amount)
    {
        if (amount <= 0) return;
        if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(AnimateMoneyChange(currentMoney, currentMoney + amount));
        currentMoney += amount;
    }

    public void SubtractMoney(double amount)
    {
        if (amount <= 0) return;
        double newMoney = Mathf.Max(0, (float)currentMoney - (float)amount); // Không cho tiền âm
        currentMoney = newMoney;
        if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(AnimateMoneyChange(currentMoney, newMoney));
        //currentMoney = newMoney;
        Debug.Log($"[SubtractMoney] New Money: {currentMoney}");
    }

    private IEnumerator AnimateMoneyChange(double from, double to)
    {
        float duration = 1f; // thời gian hiệu ứng
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            double t = Mathf.Clamp01(elapsed / duration);
            double displayMoney = Math.Round(from + (to - from) * t);
            UpdateMoneyText(displayMoney);
            yield return null;
        }

        UpdateMoneyText(to);
    }

    private void UpdateMoneyText(double value)
    {
        if (value < 1000)
        {
            textMoney.text = value.ToString();
        }
        else
        {
            textMoney.text = value.ToString("N0").Replace(",", ".");
        }
    }

    #endregion

    #region Popups

    private void ClickButtonShop()
    {
        shop.gameObject.SetActive(true);
    }

    #endregion
}
