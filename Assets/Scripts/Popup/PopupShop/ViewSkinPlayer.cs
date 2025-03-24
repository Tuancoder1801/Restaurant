using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSkinPlayer : MonoBehaviour
{
    public Transform content;
    public SkinPlayerItem skin;

    private List<SkinPlayerItem> skinPlayers = new List<SkinPlayerItem>();

    private void Awake()
    {
        CreateSkinItems();
    }

    private void CreateSkinItems()
    {
        List<SkinData> skinDatas = GameDataConstant.skins;

        foreach (var skinData in skinDatas)
        {
            foreach (var player in skinData.skinPlayer)
            {
                SkinPlayerItem skinItem = Instantiate(skin, content);
                skinItem.Load(player);
                skinPlayers.Add(skinItem);
            }
        }
    }
}
