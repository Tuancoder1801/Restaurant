using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData-", menuName = "ScriptableObjects/MapData")]
public class MapData : ScriptableObject
{
    public int MapId;
    public string nameMap;
    public List<Location> locations;
}

[Serializable]
public class Location
{
    public LocationId locationId;
    //public string name;
    public Sprite icon;
    public float price;
}
