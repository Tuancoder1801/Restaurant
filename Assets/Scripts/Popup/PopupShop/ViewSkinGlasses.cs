using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSkinGlasses : MonoBehaviour
{
    public Transform content;
    public SkinGlassesItem skin;

    private SkinGlassesId selectingId;
    private List<SkinGlassesItem> skinGlasses = new List<SkinGlassesItem>();

    private void Awake()
    {
        CreateSkinItems();
    }

    private void CreateSkinItems()
    {
        List<SkinGlasses> skinDatas = GameDataConstant.skin.skinGlasses;

        foreach (var glasses in skinDatas)
        {
            SkinGlassesItem skinItem = Instantiate(skin, content);
            skinItem.Load(glasses);
            skinGlasses.Add(skinItem);
        }
    }

    public void Select(SkinGlassesId id)
    {
        if (selectingId != id || selectingId == SkinGlassesId.None)
        {
            selectingId = id;
            Highlight();
            UpdateSkin();
        }
    }

    private void Highlight()
    {
        for (int i = 0; i < skinGlasses.Count; i++)
        {
            skinGlasses[i].SetTick(selectingId == skinGlasses[i].id);
        }
    }

    private void UpdateSkin()
    {
        var skinData = GameDataConstant.skin.skinGlasses;

        for (int i = 0; i < skinData.Count; i++)
        {
            if (skinData[i].id == selectingId)
            {
                ShopAreaController.Instance.shopCharacter.LoadGlass(selectingId);
            }
        }
    }
}
