using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataMoney : MonoBehaviour
{
    public void SaveCoins(double currentCoins)
    {
        PlayerPrefs.SetString(UserData.USER_DATA_MONEY, currentCoins.ToString());
        PlayerPrefs.Save();
    }

    public double LoadCoins()
    {
        string value = PlayerPrefs.GetString(UserData.USER_DATA_MONEY, "100");
        double.TryParse(value, out double result);
        return result;
    }
}
