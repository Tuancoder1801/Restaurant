using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSkinRobot : MonoBehaviour
{
    public Transform content;
    public SkinRobotItem skin;

    private List<SkinRobotItem> skinRobots = new List<SkinRobotItem>();

    private void Awake()
    {
        CreateSkinItems();
    }

    private void CreateSkinItems()
    {
        List<SkinData> skinDatas = GameDataConstant.skins;

        foreach (var skinData in skinDatas)
        {
            foreach (var robot in skinData.skinRobot)
            {
                SkinRobotItem skinItem = Instantiate(skin, content);
                skinItem.Load(robot);
                skinRobots.Add(skinItem);
            }
        }
    }
}
