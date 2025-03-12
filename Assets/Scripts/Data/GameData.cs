using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{   
    public static GameData Instance;

    public int currentMapIndex = 0;
   
    public MapData currentMap;

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
}

public class UserData
{
    public double money;
}