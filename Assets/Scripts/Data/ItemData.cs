using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData-", menuName = "ScriptableObjects/ClothesData")]
public class ItemData : ScriptableObject
{
    public ItemId itemId;
    public Sprite icon;
}
