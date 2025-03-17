using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VKTextValueChange : MonoBehaviour
{
    public TMPro.TMP_Text textNumber;

    public string strMoneyDot;

    public bool isMoney;
    public bool isSubMoney;

    public float timeRun = 1f;
    public float minSubMoney = 0f;

    public float targetNumber;
    private float number;
    private LTDescr ltDescr;

    public void SetNumber(double num)
    {
        SetNumber((float)num);
    }

    public void SetNumber(float num)
    {
        if (ltDescr != null)
        {
            LeanTween.cancel(ltDescr.id);
            ltDescr = null;
        }

        this.number = num;
        this.targetNumber = num;

        ShowText(num);
    }

    private void ShowText(float num)
    {
        if (isMoney)
        {
            if (isSubMoney && num >= minSubMoney)
            {
                textNumber.text = VKCommon.ConvertSubMoneyString(num);
            }
            else
            {
                textNumber.text = VKCommon.ConvertStringMoney(num, strMoneyDot);
            }
        }
        else
        {
            textNumber.text = num.ToString();
        }
    }

    public void UpdateNumber(double newNumber)
    {
        UpdateNumber((float)newNumber);
    }

    public void UpdateNumber(float newNumber)
    {
        this.targetNumber = newNumber;

        if (ltDescr != null)
        {
            LeanTween.cancel(ltDescr.id);
            ltDescr = null;
        }

        ltDescr = LeanTween.value(gameObject, UpdateNewValue, number, newNumber, timeRun).setOnComplete(() =>
        {
            ltDescr = null;
            number = newNumber;
            ShowText(number);
        }).setIgnoreTimeScale(true);
    }

    private void UpdateNewValue(float newNumber)
    {
        number = newNumber;
        ShowText(number);
    }
}
