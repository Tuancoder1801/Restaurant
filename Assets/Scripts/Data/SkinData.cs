using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SkinData-", menuName = "ScriptableObjects/SkinData")]
public class SkinData : ScriptableObject
{
    public List<SkinPlayer> skinPlayer;
    public List<SkinGlasses> skinGlasses;
    public List<SkinRobot> skinRobot;
}

[System.Serializable]
public class SkinPlayer
{
    public SkinPlayerId id;
    public Sprite icon;
    public int price;
}

[System.Serializable]
public class SkinGlasses
{
    public SkinGlassesId id;
    public Sprite icon;
    public int price;
}

[System.Serializable]
public class SkinRobot
{
    public SkinRobotId id;
    public Sprite icon;
    public int price;
}
