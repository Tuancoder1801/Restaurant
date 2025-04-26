using System.Collections;
using UnityEngine;

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
            skin = string.IsNullOrEmpty(skinPrefs) ? new UserDataSkin() : JsonUtility.FromJson<UserDataSkin>(skinPrefs);
        }

        if (map == null)
        {
            string mapPrefs = PlayerPrefs.GetString(USER_DATA_MAP);
            map = string.IsNullOrEmpty(mapPrefs) ? new UserDataMap() : JsonUtility.FromJson<UserDataMap>(mapPrefs);
            map.InitDefault();
        }

        if (PlayerPrefs.HasKey("last_map_index"))
        {
            map.currentMapIndex = PlayerPrefs.GetInt("last_map_index");
        }

        if (money == null)
        {
            money = new UserDataMoney();
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(map);
        PlayerPrefs.SetString(USER_DATA_MAP, json);
        PlayerPrefs.SetInt("last_map_index", map.currentMapIndex);
        PlayerPrefs.Save();
    }

    public static void PrepareMapSave(int mapId)
    {
        map.UnlockMap(mapId);

        if (!map.HasMapData(mapId))
        {
            var data = new UserMapData();
            map.allMapData.Add(new MapDataEntry { mapIndex = mapId, data = data });
            Save();
        }
    }

    public static void CompleteCurrentMap()
    {
        map.CompleteMap(map.currentMapIndex);
        Save();
    }
}
