using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserDataSkin
{
    public List<SkinEquipEntry> equippedSkins = new List<SkinEquipEntry>();
    public List<SkinOwnedEntry> ownedSkins = new List<SkinOwnedEntry>();

    [Serializable]
    public class SkinEquipEntry
    {
        public int skinType;   // int = (int)SkinType
        public int skinId;
    }

    [Serializable]
    public class SkinOwnedEntry
    {
        public int skinType;
        public List<int> ownedIds = new List<int>();
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(UserData.USER_DATA_SKIN, json);
        PlayerPrefs.Save();
    }

    public int GetEquippedSkin(SkinType type)
    {
        int key = (int)type;
        foreach (var entry in equippedSkins)
        {
            if (entry.skinType == key)
                return entry.skinId;
        }

        return 0; // default skin
    }

    public List<int> GetOwnedSkins(SkinType type)
    {
        int key = (int)type;
        foreach (var entry in ownedSkins)
        {
            if (entry.skinType == key)
                return entry.ownedIds;
        }

        return new List<int>();
    }

    public void Buy(SkinType type, int id)
    {
        int key = (int)type;
        var entry = ownedSkins.Find(e => e.skinType == key);
        if (entry == null)
        {
            entry = new SkinOwnedEntry { skinType = key };
            ownedSkins.Add(entry);
        }

        if (!entry.ownedIds.Contains(id))
        {
            entry.ownedIds.Add(id);
            Save();
        }
    }

    public void Equip(SkinType type, int id)
    {
        int key = (int)type;
        var owned = GetOwnedSkins(type);
        if (!owned.Contains(id)) return;

        var equip = equippedSkins.Find(e => e.skinType == key);
        if (equip == null)
        {
            equip = new SkinEquipEntry { skinType = key };
            equippedSkins.Add(equip);
        }

        equip.skinId = id;
        Save();
    }
}
