using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ViewSkinPlayer : MonoBehaviour
{
    public Transform content;
    public SkinPlayerItem skin;

    private SkinPlayerId selectingId;
    private List<SkinPlayerItem> skinPlayers = new List<SkinPlayerItem>();
    private List<int> ownedSkinPlayers = new List<int>();

    private void OnEnable()
    {
        CreateSkinItems();

        if (ownedSkinPlayers.Contains(UserData.skin.GetEquippedSkin(SkinType.Set)))
        {
            Select((SkinPlayerId)UserData.skin.GetEquippedSkin(SkinType.Set));
        }
    }

    private void CreateSkinItems()
    {
        List<SkinPlayer> skinDatas = GameDataConstant.skin.skinPlayer;

        foreach (var player in skinDatas)
        {
            SkinPlayerItem skinItem = Instantiate(skin, content);
            skinItem.Load(player);
            skinPlayers.Add(skinItem);
        }
    }

    public void Select(SkinPlayerId id)
    {
        if (selectingId != id || selectingId == SkinPlayerId.None)
        {
            selectingId = id;
            UpdateSkin();
        }
    }

    private void UpdateSkin()
    {
        var skinData = GameDataConstant.skin.skinPlayer;

        for (int i = 0; i < skinData.Count; i++)
        {
            if (skinData[i].id == selectingId)
            {
                if (ownedSkinPlayers.Contains((int)skinData[i].id))
                {
                    skin.SetTick(false);
                    skin.SetPrice(false);

                    if ((int)skinData[i].id == UserData.skin.GetEquippedSkin(SkinType.Set))
                    {
                        skin.SetTick(true);
                    }
                    else
                    {
                        skin.SetTick(false);
                    }
                }
                else
                {
                    skin.SetPrice(true);
                    skin.SetTick(false);
                }

                ShopAreaController.Instance.shopCharacter.LoadCharacter(skinData[i].id);
                GameManager.Instance.player.EquipSkinPlayer(skinData[i].id);
            }
        }
    }

    public void BuySkin()
    {
        var skinData = GameDataConstant.skin.skinPlayer;
        for (int i = 0; i < skinData.Count; i++)
        {
            if (skinData[i].id == selectingId)
            {
                var price = skinData[i].price;

                if(UIGame.Instance.currentMoney >= price)
                {
                    UIGame.Instance.SubtractMoney(price);
                }

                if (!ownedSkinPlayers.Contains((int)skinData[i].id))
                {
                    ownedSkinPlayers.Add((int)skinData[i].id);
                    skin.SetTick(true);
                    skin.SetPrice(false);

                    UserData.skin.Buy(SkinType.Set, (int)skinData[i].id);
                    EquipSkin();
                }
            }
        }
    }

    private void EquipSkin()
    {
        var skinData = GameDataConstant.skin.skinPlayer;
        for (int i = 0; i < skinData.Count; i++)
        {
            if (skinData[i].id == selectingId)
            {   
                ShopAreaController.Instance.shopCharacter.LoadCharacter(skinData[i].id);    
                GameManager.Instance.player.EquipSkinPlayer(skinData[i].id);
                skin.SetTick(true);
                skin.SetPrice(false);

                UserData.skin.Equip(SkinType.Set, (int)skinData[i].id);
                //UpdateHatInfo();
            }
        }
    }
}
