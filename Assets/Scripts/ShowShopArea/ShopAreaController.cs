using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAreaController : Singleton<ShopAreaController>
{
    public static ShopAreaController Instance;

    public GameObject goContent;
    public ShopCharacter shopCharacter;
    public ShopCollector shopCollector;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }

        goContent.SetActive(false);
    }

    public void ShowShop()
    {
        goContent.SetActive(true);
        InitSkin();
    }

    public void Hide()
    {
        goContent.SetActive(false);
    }

    public void InitSkin()
    {
        int equippedPlayerId = UserData.skin.GetEquippedSkin(SkinType.Set);
        int equippedGlassId = UserData.skin.GetEquippedSkin(SkinType.Glass);
        int equippedCollectorId = UserData.skin.GetEquippedSkin(SkinType.Robot);

        shopCharacter.LoadCharacter((SkinPlayerId)equippedPlayerId);   
        shopCharacter.LoadGlass((SkinGlassesId)equippedGlassId);
        shopCollector.LoadCollector((SkinRobotId)equippedCollectorId);
    }
}
