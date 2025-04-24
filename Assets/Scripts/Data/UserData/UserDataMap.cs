using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserMapData
{
    public List<int> unlockedBuildIndexes = new List<int>();
    public Dictionary<int, int> customersPerBuild = new Dictionary<int, int>();
}

[System.Serializable]
public class UserDataMap
{
    public int currentMapIndex;

    public List<int> unlockedMapIndexes = new List<int>();
    public List<int> completedMapIndexes = new List<int>();

    public Dictionary<int, UserMapData> allMapData = new Dictionary<int, UserMapData>();

    public void InitDefault()
    {
        if(unlockedMapIndexes == null || unlockedMapIndexes.Count == 0)
        {
            unlockedMapIndexes = new List<int> { 0 };
        }

        if (!allMapData.ContainsKey(0))
        {
            allMapData[0] = new UserMapData();
            allMapData[0].unlockedBuildIndexes.Add(0);
        }
    }

    public void UnlockMap(int mapIndex)
    {
        if (!unlockedMapIndexes.Contains(mapIndex))
            unlockedMapIndexes.Add(mapIndex);

        if (!allMapData.ContainsKey(mapIndex))
            allMapData[mapIndex] = new UserMapData();
    }

    public void CompleteMap(int mapIndex)
    {
        if (!completedMapIndexes.Contains(mapIndex))
            completedMapIndexes.Add(mapIndex);
    }

    public bool IsMapUnlocked(int mapIndex) => unlockedMapIndexes.Contains(mapIndex);
    public bool IsMapCompleted(int mapIndex) => completedMapIndexes.Contains(mapIndex);
    public bool IsCurrentMap(int mapIndex) => currentMapIndex == mapIndex;
}

