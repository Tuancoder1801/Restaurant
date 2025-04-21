using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSkinGlasses : MonoBehaviour
{
    public Transform content;
    public SkinGlassesItem skinPrefab;

    private SkinGlassesId selectingId;
    private List<SkinGlassesItem> skinGlasses = new List<SkinGlassesItem>();

    private void OnEnable()
    {
        EnsureDefaultSkin();

        CreateSkinItems();

        int equipped = UserData.skin.GetEquippedSkin(SkinType.Glass);
        Select((SkinGlassesId)equipped);
    }

    private void EnsureDefaultSkin()
    {
        var owned = UserData.skin.GetOwnedSkins(SkinType.Glass);
        var equipped = UserData.skin.GetEquippedSkin(SkinType.Glass);

        if (owned == null || owned.Count == 0 || !owned.Contains(equipped))
        {
            UserData.skin.Buy(SkinType.Glass, 0);
            UserData.skin.Equip(SkinType.Glass, 0);
        }
    }

    private void CreateSkinItems()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
        skinGlasses.Clear();

        var skinDatas = GameDataConstant.skin.skinGlasses;

        foreach (var data in skinDatas)
        {
            var item = Instantiate(skinPrefab, content);
            item.Load(data);
            item.SetTick(false);
            item.SetPrice(true);
            item.OnClick(() => Select(data.id));
            skinGlasses.Add(item);
        }
    }

    public void Select(SkinGlassesId id)
    {
        selectingId = id;

        var skinData = GameDataConstant.skin.skinGlasses.Find(s => s.id == id);
        if (skinData == null) return;

        bool isOwned = UserData.skin.GetOwnedSkins(SkinType.Glass).Contains((int)id);

        if (isOwned)
        {
            EquipSkin(id);
        }
        else
        {
            if (UIGame.Instance.currentMoney >= skinData.price)
            {
                UIGame.Instance.SubtractMoney(skinData.price);
                UserData.skin.Buy(SkinType.Glass, (int)id);
                EquipSkin(id);
            }
            else
            {
                Debug.Log("Không đủ tiền mua skin.");
            }
        }

        UpdateSkinUI();
    }

    private void EquipSkin(SkinGlassesId id)
    {
        UserData.skin.Equip(SkinType.Glass, (int)id);
        GameManager.Instance.player.EquipSkinGlass(id);
        ShopAreaController.Instance.shopCharacter.LoadGlass(id);
    }

    private void UpdateSkinUI()
    {
        var owned = UserData.skin.GetOwnedSkins(SkinType.Glass);
        var equipped = UserData.skin.GetEquippedSkin(SkinType.Glass);

        foreach (var item in skinGlasses)
        {
            bool isOwned = owned.Contains((int)item.id);
            bool isEquipped = equipped == (int)item.id;

            item.SetTick(isEquipped);
            item.SetPrice(!isOwned);
        }
    }
}
