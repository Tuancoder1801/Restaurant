using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataMoney : MonoBehaviour
{
    public void SaveCoins(float currentCoins)
    {
        PlayerPrefs.SetFloat(UserData.USER_DATA_MONEY, currentCoins);
        PlayerPrefs.Save();
    }

    public float LoadCoins()
    {
        return PlayerPrefs.GetFloat(UserData.USER_DATA_MONEY, 0);
    }
}
