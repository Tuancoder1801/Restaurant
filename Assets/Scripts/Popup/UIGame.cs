using System;
using System.Collections;
using TMPro;
using UnityEngine;
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

    private void OnEnable()
    {
        Debug.Log("✅ OnEnable " + nameof(gameObject));

        LoadMoney();
        bg.gameObject.SetActive(false);
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

        UserData.money.SaveCoins(currentMoney);
    }

    public void SubtractMoney(double amount)
    {
        if (amount <= 0) return;
        double newMoney = Mathf.Max(0, (float)currentMoney - (float)amount); // Không cho tiền âm
        currentMoney = newMoney;
        if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(AnimateMoneyChange(currentMoney, newMoney));

        UserData.money.SaveCoins(currentMoney);
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

    private void LoadMoney()
    {
        double currentCoins = UserData.money.LoadCoins();
        Debug.Log("💰 LoadCoins = " + currentCoins + " | Raw = " + PlayerPrefs.GetString(UserData.USER_DATA_MONEY));
        currentMoney = currentCoins;    

        if (currentCoins < 1000)
        {
            textMoney.text = currentCoins.ToString();
        }
        else
        {
            textMoney.text = currentCoins.ToString("N0").Replace(",", ".");
        }
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
        Debug.Log("🛒 ClickButtonShop called!");
        popupShop.gameObject.SetActive(true);
        AudioManager.Instance.TurnOnButtonSound();
        HideButtonShop();
    }

    private void ClickButtonSetting()
    {
        Debug.Log("🛒 ClickButtonSetting called!");
        popupSetting.gameObject.SetActive(true);
        AudioManager.Instance.TurnOnButtonSound();
        HideButtons();
    }

    private void ClickButtonMart()
    {
        Debug.Log("🛒 ClickButtonMart called!");
        popupMart.gameObject.SetActive(true);
        AudioManager.Instance.TurnOnButtonSound();
        HideButtons();
    }

    private void HideButtons()
    {
        bg.gameObject.SetActive(true);
        btSetting.gameObject.SetActive(false);
        btShop.gameObject.SetActive(false);
        btMart.gameObject.SetActive(false);
    }

    private void HideButtonShop()
    {
        bg.gameObject.SetActive(false);
        btSetting.gameObject.SetActive(false);
        btShop.gameObject.SetActive(false);
        btMart.gameObject.SetActive(false);
    }

    public void HidePopups()
    {
        ShowButtons();
        popupSetting.gameObject.SetActive(false);
        popupMart.gameObject.SetActive(false);
        popupShop.gameObject.SetActive(false);  
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
