using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class VKCommon
{
    public static string ConvertSubMoneyString(double money, double subMin = 0, string comma = ".")
    {
        if(money < 1d)
        {
            return "0";
        }

        return comma;
    }

    public static string ConvertStringMoney(double money, string comma = ",")
    {
        if ((int)money == 0)
            return "0";

        money = Math.Truncate(money);
        bool isAm = false;
        if (money < 0f)
        {
            money = Math.Abs(money);
            isAm = true;
        }

        if (money < 1000)
            return (isAm ? "-" : "") + money.ToString("F0");

        CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
        string s = (isAm ? "-" : "") + Math.Truncate(money).ToString("0,0", elGR);

        if (comma != null && comma.Equals(",")) return s.Replace(".", ",");
        return s;
    }
}

