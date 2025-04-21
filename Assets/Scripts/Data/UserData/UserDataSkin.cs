using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataSkin
{
    public Dictionary<int, int> equippedSkins = new Dictionary<int, int>();
    public Dictionary<int, List<int>> ownedSkins = new Dictionary<int, List<int>>();

    private void Save()
    {
        string json = JsonConvert.SerializeObject(this);
        PlayerPrefs.SetString(UserData.USER_DATA_SKIN, json);
        PlayerPrefs.Save();
    }

    public int GetEquippedSkin(SkinType type)
    {
        int key = (int)type;
        if (equippedSkins.ContainsKey(key))
        {
            return equippedSkins[key];
        }

        return 0;
    }

    public List<int> GetOwnedSkins(SkinType type)
    {
        int key = (int)type;
        if (ownedSkins.ContainsKey(key))
        {
            return ownedSkins[key];
        }
        return new List<int>();
    }

    public void Buy(SkinType type, int id)
    {
        List<int> owned = GetOwnedSkins(type);
        if (owned.Contains(id) == false)
        {
            owned.Add(id);
            ownedSkins[(int)type] = owned;
            Save();
        }

    }

    public void Equip(SkinType type, int id)
    {
        int typeInt = (int)type;

        if (ownedSkins.ContainsKey(typeInt) && ownedSkins[typeInt].Contains(id))
        {
            equippedSkins[typeInt] = id;
            Save();
        }
    }
}
