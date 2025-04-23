using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{   
    public static GameData Instance;

    public int currentMapIndex = 0;
    public static MapData currentMap;

    private bool isLoadingMap = false;  

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameDataConstant.Load();
            UserData.Load();
            Debug.Log("UserData loaded", this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        currentMapIndex = UserData.map.currentMapIndex;

        if (SceneManager.GetActiveScene().name == "Start")
        {
            WaitAndLoadMap();
        }
        else
        {
            LoadCurrentMap();
        }
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
        int next = UserData.map.currentMapIndex + 1;
        if (next < GameDataConstant.maps.Count)
        {
            UserData.map.currentMapIndex = next;
            UserData.PrepareMapSave(next);
            UserData.Save(); 

            SceneManager.LoadScene("Start");
        }
    }

    private void WaitAndLoadMap()
    {
        if (isLoadingMap)
        {
            isLoadingMap = false;
            LoadNextMap();
        }
    }

    private void LoadNextMap()
    {
        string nextMapName = GetNameMap();
        if (!string.IsNullOrEmpty(nextMapName))
        {
            SceneManager.LoadScene(nextMapName);
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
        return GameDataConstant.maps[UserData.map.currentMapIndex].nameMap;
    }

    #endregion
}