using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UserData
{
    public static UserDataMoney money;
    public static UserDataSkin skin;
    public static UserDataMap map;

    public const string USER_DATA_SKIN = "user_data_skin";
    public const string USER_DATA_MONEY = "user_data_money";
    public const string USER_DATA_MAP = "user_data_map";

    public static void Load()
    {
        if (skin == null)
        {
            string skinPrefs = PlayerPrefs.GetString(USER_DATA_SKIN);
            if (string.IsNullOrEmpty(skinPrefs))
            {
                skin = new UserDataSkin();
            }
            else
            {
                skin = JsonConvert.DeserializeObject<UserDataSkin>(skinPrefs);
            }
        }

        if (map == null)
        {
            string mapPrefs = PlayerPrefs.GetString(USER_DATA_MAP);
            if (string.IsNullOrEmpty(mapPrefs))
            {
                map = new UserDataMap();
                map.unlockedBuildIndexes.Add(-1);
                Save();
            }
            else
            {
                map = JsonConvert.DeserializeObject<UserDataMap>(mapPrefs);
            }
        }

        if (money == null)
        {
            money = new UserDataMoney(); // Khởi tạo UserDataCoins nếu chưa được tạo
        }
    }

    public static void Save()
    {
        string json = JsonConvert.SerializeObject(map);
        PlayerPrefs.SetString(USER_DATA_MAP, json);
        PlayerPrefs.Save();
    }
}
