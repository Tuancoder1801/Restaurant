using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCharacter : MonoBehaviour
{
    public List<PlayerEquipment> skinPlayers = new List<PlayerEquipment>();
    public List<Glass> skinGlasses = new List<Glass>();

    private PlayerEquipment equipment;
    private GameObject currentGlassInstance;

    public void LoadCharacter(SkinPlayerId id)
    {
        for (int i = 0; i < skinPlayers.Count; i++) 
        {
            if (skinPlayers[i].id == id)
            {
                skinPlayers[i].gameObject.SetActive(true);
                equipment = skinPlayers[i].GetComponent<PlayerEquipment>();
            }
            else 
            {
                skinPlayers[i].gameObject.SetActive(false);
            }
        }
    }

    public void LoadGlass(SkinGlassesId id)
    {
        if (currentGlassInstance != null)
        {
            Destroy(currentGlassInstance);
            currentGlassInstance = null;
        }

        for (int i = 0; i < skinGlasses.Count; i++)
        {
            if (skinGlasses[i].skinGlassesId == id)
            {
                if (equipment != null && equipment.tranGlass != null)
                {
                    currentGlassInstance = Instantiate(
                        skinGlasses[i].gameObject,
                        equipment.tranGlass.position,
                        equipment.tranGlass.rotation,
                        equipment.tranGlass
                    );
                }
            }
        }
    }
}
