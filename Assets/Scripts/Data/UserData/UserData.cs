using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UserData
{
    public static UserDataMoney money;
    public static UserDataSkin skin;

    public const string USER_DATA_SKIN = "user_data_skin";
    public const string USER_DATA_MONEY = "user_data_money";

    public static void Load()
    {
        if (skin == null)
        {
            string weaponPrefs = PlayerPrefs.GetString(USER_DATA_SKIN);
            if (string.IsNullOrEmpty(weaponPrefs))
            {
                skin = new UserDataSkin();
            }
            else
            {
                skin = JsonConvert.DeserializeObject<UserDataSkin>(weaponPrefs);
            }

           // Debug.Log("weapon=" + JsonConvert.SerializeObject(weapon));
        }

        if (money == null)
        {
            money = new UserDataMoney(); // Khởi tạo UserDataCoins nếu chưa được tạo
        }
    } 
}
