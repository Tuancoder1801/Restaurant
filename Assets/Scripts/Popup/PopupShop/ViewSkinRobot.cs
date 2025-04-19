using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSkinRobot : MonoBehaviour
{
    public Transform content;
    public SkinRobotItem skin;

    private SkinRobotId selectingId;
    private List<SkinRobotItem> skinRobots = new List<SkinRobotItem>();

    private void Awake()
    {
        CreateSkinItems();
    }

    private void OnEnable()
    {
        Select(SkinRobotId.None);
    }

    private void CreateSkinItems()
    {
        List<SkinRobot> skinDatas = GameDataConstant.skin.skinRobot;

        foreach (var robot in skinDatas)
        {
            SkinRobotItem skinItem = Instantiate(skin, content);
            skinItem.Load(robot);
            skinRobots.Add(skinItem);
        }
    }

    public void Select(SkinRobotId id)
    {
        if (selectingId != id || selectingId == SkinRobotId.None)
        {
            selectingId = id;
            Highlight();
            UpdateSkin();
        }
    }

    private void Highlight()
    {
        for (int i = 0; i < skinRobots.Count; i++)
        {
            skinRobots[i].SetTick(selectingId == skinRobots[i].id);
        }
    }

    private void UpdateSkin()
    {
        var skinData = GameDataConstant.skin.skinRobot;

        for (int i = 0; i < skinData.Count; i++)
        {
            if (skinData[i].id == selectingId)
            {
                ShopAreaController.Instance.shopCollector.LoadCollector(selectingId);
            }
        }
    }
}
