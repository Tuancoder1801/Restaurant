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
    public Dictionary<int, UserMapData> allMapData = new Dictionary<int, UserMapData>();
}

