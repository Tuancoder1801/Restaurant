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
    
    private int currentMoney = 0;
    private Coroutine moneyCoroutine;

    private void Awake()
    {
        UpdateMoneyText(currentMoney);
        btShop.onClick.AddListener(ClickButtonShop);
    }

    #region money

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(AnimateMoneyChange(currentMoney, currentMoney + amount));
        currentMoney += amount;
    }

    public void SubtractMoney(int amount)
    {
        if (amount <= 0) return;
        int newMoney = Mathf.Max(0, currentMoney - amount); // Không cho tiền âm
        if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(AnimateMoneyChange(currentMoney, newMoney));
        currentMoney = newMoney;
    }

    private IEnumerator AnimateMoneyChange(int from, int to)
    {
        float duration = 1f; // thời gian hiệu ứng
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int displayMoney = Mathf.RoundToInt(Mathf.Lerp(from, to, elapsed / duration));
            UpdateMoneyText(displayMoney);
            yield return null;
        }

        UpdateMoneyText(to);
    }

    private void UpdateMoneyText(int value)
    {
        if (value < 1000)
        {
            textMoney.text = value.ToString();
        }
        else
        {
            textMoney.text = value.ToString("N0").Replace(",", "."); // Đổi dấu ',' thành '.'
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
