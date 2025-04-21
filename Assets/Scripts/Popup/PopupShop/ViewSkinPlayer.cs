using System.Collections.Generic;
using UnityEngine;

public class ViewSkinPlayer : MonoBehaviour
{
    public Transform content;
    public SkinPlayerItem skinPrefab;

    private SkinPlayerId selectingId;
    private List<SkinPlayerItem> skinPlayers = new List<SkinPlayerItem>();

    private void OnEnable()
    {
        EnsureDefaultSkin();

        CreateSkinItems();

        int equipped = UserData.skin.GetEquippedSkin(SkinType.Set);
        Select((SkinPlayerId)equipped);
    }

    private void EnsureDefaultSkin()
    {
        var owned = UserData.skin.GetOwnedSkins(SkinType.Set);
        var equipped = UserData.skin.GetEquippedSkin(SkinType.Set);

        if (owned == null || owned.Count == 0 || !owned.Contains(equipped))
        {
            UserData.skin.Buy(SkinType.Set, 0);
            UserData.skin.Equip(SkinType.Set, 0);
        }
    }

    private void CreateSkinItems()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
        skinPlayers.Clear();

        var skinDatas = GameDataConstant.skin.skinPlayer;

        foreach (var data in skinDatas)
        {
            var item = Instantiate(skinPrefab, content);
            item.Load(data);
            item.SetTick(false);
            item.SetPrice(true);
            item.OnClick(() => Select(data.id));
            skinPlayers.Add(item);
        }
    }

    public void Select(SkinPlayerId id)
    {
        selectingId = id;

        var skinData = GameDataConstant.skin.skinPlayer.Find(s => s.id == id);
        if (skinData == null) return;

        bool isOwned = UserData.skin.GetOwnedSkins(SkinType.Set).Contains((int)id);

        if (isOwned)
        {
            EquipSkin(id);
        }
        else
        {
            if (UIGame.Instance.currentMoney >= skinData.price)
            {
                UIGame.Instance.SubtractMoney(skinData.price);
                UserData.skin.Buy(SkinType.Set, (int)id);
                EquipSkin(id);
            }
            else
            {
                Debug.Log("Không đủ tiền mua skin.");
            }
        }

        UpdateSkinUI();
    }

    private void EquipSkin(SkinPlayerId id)
    {
        UserData.skin.Equip(SkinType.Set, (int)id);
        GameManager.Instance.player.EquipSkinPlayer(id);
        ShopAreaController.Instance.shopCharacter.LoadCharacter(id);
    }

    private void UpdateSkinUI()
    {
        var owned = UserData.skin.GetOwnedSkins(SkinType.Set);
        var equipped = UserData.skin.GetEquippedSkin(SkinType.Set);

        foreach (var item in skinPlayers)
        {
            bool isOwned = owned.Contains((int)item.id);
            bool isEquipped = equipped == (int)item.id;

            item.SetTick(isEquipped);
            item.SetPrice(!isOwned);
        }
    }
}
