using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPosition : MonoBehaviour
{
    public ItemId itemId;
    public List<Transform> itemPositions;
    public int currentStackNumber = 0;
}
