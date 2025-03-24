using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSkinGlasses : MonoBehaviour
{
    public Transform content;
    public SkinGlassesItem skin;

    private List<SkinGlassesItem> skinGlasses = new List<SkinGlassesItem>();

    private void Awake()
    {
        CreateSkinItems();
    }

    private void CreateSkinItems()
    {
        List<SkinData> skinDatas = GameDataConstant.skins;

        foreach (var skinData in skinDatas)
        {
            foreach (var glasses in skinData.skinGlasses)
            {
                SkinGlassesItem skinItem = Instantiate(skin, content);
                skinItem.Load(glasses);
                skinGlasses.Add(skinItem);
            }
        }
    }
}
