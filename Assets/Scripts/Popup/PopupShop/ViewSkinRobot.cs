using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSkinRobot : MonoBehaviour
{
    public Transform content;
    public SkinRobotItem skinPrefab;

    private SkinRobotId selectingId;
    private List<SkinRobotItem> skinRobots = new List<SkinRobotItem>();

    private void OnEnable()
    {
        EnsureDefaultSkin();

        CreateSkinItems();

        int equipped = UserData.skin.GetEquippedSkin(SkinType.Robot);
        Select((SkinRobotId)equipped);
    }

    private void EnsureDefaultSkin()
    {
        var owned = UserData.skin.GetOwnedSkins(SkinType.Robot);
        var equipped = UserData.skin.GetEquippedSkin(SkinType.Robot);

        if (owned == null || owned.Count == 0 || !owned.Contains(equipped))
        {
            UserData.skin.Buy(SkinType.Robot, 0);
            UserData.skin.Equip(SkinType.Robot, 0);
        }
    }

    private void CreateSkinItems()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
        skinRobots.Clear();

        var skinDatas = GameDataConstant.skin.skinRobot;

        foreach (var data in skinDatas)
        {
            var item = Instantiate(skinPrefab, content);
            item.Load(data);
            item.SetTick(false);
            item.SetPrice(true);
            item.OnClick(() => Select(data.id));
            skinRobots.Add(item);
        }
    }

    public void Select(SkinRobotId id)
    {
        selectingId = id;

        var skinData = GameDataConstant.skin.skinRobot.Find(s => s.id == id);
        if (skinData == null) return;

        bool isOwned = UserData.skin.GetOwnedSkins(SkinType.Robot).Contains((int)id);

        if (isOwned)
        {
            EquipSkin(id);
        }
        else
        {
            if (UIGame.Instance.currentMoney >= skinData.price)
            {
                UIGame.Instance.SubtractMoney(skinData.price);
                UserData.skin.Buy(SkinType.Robot, (int)id);
                EquipSkin(id);
            }
            else
            {
                Debug.Log("Không đủ tiền mua skin.");
            }
        }

        UpdateSkinUI();
    }

    private void EquipSkin(SkinRobotId id)
    {
        UserData.skin.Equip(SkinType.Robot, (int)id);
        GameManager.Instance.collector.EquipSkinCollector(id);
        ShopAreaController.Instance.shopCollector.LoadCollector(id);
    }

    private void UpdateSkinUI()
    {
        var owned = UserData.skin.GetOwnedSkins(SkinType.Robot);
        var equipped = UserData.skin.GetEquippedSkin(SkinType.Robot);

        foreach (var item in skinRobots)
        {
            bool isOwned = owned.Contains((int)item.id);
            bool isEquipped = equipped == (int)item.id;

            item.SetTick(isEquipped);
            item.SetPrice(!isOwned);
        }
    }
}
