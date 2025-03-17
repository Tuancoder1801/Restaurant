using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{   
    public static GameData Instance;

    public int currentMapIndex = 0;
   
    public static MapData currentMap;
    public static UserData user;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        GameDataConstant.Load();

        LoadCurrentMap();
    }

    #region DataMap
    private void LoadCurrentMap()
    {
        if(currentMapIndex >= 0 && currentMapIndex < GameDataConstant.maps.Count)
        {
            currentMap = GameDataConstant.maps[currentMapIndex];    
        }
    }

    public void NextMap()
    {
        if(currentMapIndex + 1 < GameDataConstant.maps.Count)
        {
            currentMapIndex++;
            LoadCurrentMap();
        }
    }

    public MapData GetCurrentMapData(int mapIndex)
    {
        if (mapIndex >= 0 && mapIndex < GameDataConstant.maps.Count)
        {
            return GameDataConstant.maps[mapIndex];
        }
        return null;
    }

    public string GetNameMap()
    {
        if (currentMapIndex >= GameDataConstant.maps.Count) return null;
        return GameDataConstant.maps[currentMapIndex].nameMap;
    }

    #endregion

    #region DataUser
        
    //public void UpdateMoney(double add)
    //{
    //    user.money += add;
    //    if(user.money < 0) user.money = 0;
    //    LeanTween.dispatchEvent(1, user.money);
    //}

    #endregion
}

public class UserData
{
    public double money;
}