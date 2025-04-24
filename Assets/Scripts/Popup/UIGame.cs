using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIGame : Singleton<UIGame>
{
    public Transform bg;

    [Header("Money")]
    public TextMeshProUGUI textMoney;

    [Header("Popups")]
    public Button btShop;
    public PopupShop popupShop;
    public Button btSetting;
    public PopupSetting popupSetting;
    public Button btMart;
    public PopupMart popupMart;

    public double currentMoney = 0;
    private Coroutine moneyCoroutine;

    private void Awake()
    {
        UpdateMoneyText(currentMoney);
        btShop.onClick.AddListener(ClickButtonShop);
        btSetting.onClick.AddListener(ClickButtonSetting);
        btMart.onClick.AddListener(ClickButtonMart);
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
        popupShop.gameObject.SetActive(true);
        HideButtons();
    }

    private void ClickButtonSetting()
    {
        popupSetting.gameObject.SetActive(true);
        HideButtons();
    }

    private void ClickButtonMart()
    {
        popupMart.gameObject.SetActive(true);
        HideButtons();
    }

    private void HideButtons()
    {
        bg.gameObject.SetActive(true);
        btSetting.gameObject.SetActive(false);
        btShop.gameObject.SetActive(false);
        btMart.gameObject.SetActive(false);
    }

    public void ShowButtons()
    {
        bg.gameObject.SetActive(false);
        btSetting.gameObject.SetActive(true);
        btShop.gameObject.SetActive(true);
        btMart.gameObject.SetActive(true);
    }

    #endregion
}
