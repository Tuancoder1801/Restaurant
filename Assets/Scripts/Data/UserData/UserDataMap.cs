using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserMapData
{
    public List<int> unlockedBuildIndexes = new List<int>();
    public List<BuildCustomerEntry> customersPerBuild = new List<BuildCustomerEntry>();

    public int GetCustomerCount(int buildIndex)
    {
        foreach (var entry in customersPerBuild)
        {
            if (entry.buildIndex == buildIndex)
                return entry.customerCount;
        }
        return 0;
    }

    public void SetCustomerCount(int buildIndex, int count)
    {
        var entry = customersPerBuild.Find(e => e.buildIndex == buildIndex);
        if (entry == null)
        {
            customersPerBuild.Add(new BuildCustomerEntry { buildIndex = buildIndex, customerCount = count });
        }
        else
        {
            entry.customerCount = count;
        }
    }

    [Serializable]
    public class BuildCustomerEntry
    {
        public int buildIndex;
        public int customerCount;
    }
}

[Serializable]
public class UserDataMap
{
    public int currentMapIndex;

    public List<int> unlockedMapIndexes = new List<int>();
    public List<int> completedMapIndexes = new List<int>();

    public List<MapDataEntry> allMapData = new List<MapDataEntry>();

    public void InitDefault()
    {
        if (unlockedMapIndexes == null || unlockedMapIndexes.Count == 0)
            unlockedMapIndexes = new List<int> { 0 };

        if (!HasMapData(0))
        {
            allMapData.Add(new MapDataEntry
            {
                mapIndex = 0,
                data = new UserMapData()
            });
        }
    }

    public void UnlockMap(int mapIndex)
    {
        if (!unlockedMapIndexes.Contains(mapIndex))
            unlockedMapIndexes.Add(mapIndex);

        if (!HasMapData(mapIndex))
            allMapData.Add(new MapDataEntry { mapIndex = mapIndex, data = new UserMapData() });
    }

    public void CompleteMap(int mapIndex)
    {
        if (!completedMapIndexes.Contains(mapIndex))
            completedMapIndexes.Add(mapIndex);
    }

    public UserMapData GetMapData(int mapIndex)
    {
        return allMapData.Find(entry => entry.mapIndex == mapIndex)?.data;
    }

    public bool HasMapData(int mapIndex)
    {
        return allMapData.Exists(entry => entry.mapIndex == mapIndex);
    }

    public bool IsBuildUnlocked(int mapIndex, int buildIndex)
    {
        var mapData = GetMapData(mapIndex);
        if (mapData != null)
        {
            return mapData.unlockedBuildIndexes.Contains(buildIndex);
        }
        return false;
    }
}

[Serializable]
public class MapDataEntry
{
    public int mapIndex;
    public UserMapData data;
}
