using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCollector : MonoBehaviour
{
    public List<CollectorEquipment> equipments = new List<CollectorEquipment>();

    public void LoadCollector(SkinRobotId id)
    {
        for (int i = 0; i < equipments.Count; i++)
        {
            if (equipments[i].skinRobotId == id)
            {
                equipments[i].gameObject.SetActive(true);
            }
            else
            {
                equipments[i].gameObject.SetActive(false);
            }
        }
    }
}
