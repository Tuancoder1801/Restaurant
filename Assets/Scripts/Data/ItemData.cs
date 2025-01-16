using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData-", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemId itemId;
    public Sprite icon;
}
