using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class VKCommon
{
    private static readonly int charA = Convert.ToInt32('a');
    private static readonly List<string> units = new List<string> { "", "K", "M", "B", "T" };

    public static string ConvertSubMoneyString(double money, double subMin = 0, string comma = ".")
    {
        if(money < 1d)
        {
            return "0";
        }

        var n = (int)Math.Log(money, 1000);

        if (n < 0)
        {
            return "0";
        }
        else if (n == 0 || money < subMin)
        {
            return ConvertStringMoney(money, comma);
        }

        var m = money / Math.Pow(1000, n);
        var unit = "";

        if (n < units.Count)
        {
            unit = units[n];
        }
        else
        {
            var unitInt = n - units.Count;
            var secondUnit = unitInt % 26;
            var firstUnit = unitInt / 26;
            unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
        }

        return (Math.Floor(m * 100) / 100).ToString("0.##") + unit;
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

