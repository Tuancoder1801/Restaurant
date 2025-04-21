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
            Debug.Log("UserData loaded", this); // trong GameData
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
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
        if(currentMapIndex + 1 < GameDataConstant.maps.Count)
        {
            currentMapIndex++;
            isLoadingMap = true;
            SceneManager.LoadScene("Start");
            //LoadCurrentMap();
        }
    }

    private void WaitAndLoadMap()
    {
        //yield return new WaitForSeconds(1f); // Chờ 2 giây trước khi load map mới

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
        if (currentMapIndex >= GameDataConstant.maps.Count) return null;
        return GameDataConstant.maps[currentMapIndex].nameMap;
    }

    #endregion
}