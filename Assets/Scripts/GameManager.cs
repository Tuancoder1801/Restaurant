using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : Singleton<GameManager>
{
    public int currentMapIndex = 0;

    [Header("Player")]
    [Space(20)]
    public Player player;
    public AICollector collector;
    public List<AICharacter> customers;
    public List<AICharacter> customerVips;

    public SmoothCamera smoothCamera;
    public BoxCollider box;

    [Header("Transform")]
    [Space(20)]
    public List<Transform> transCustomers;
    public List<Transform> transColectorWayPoints;

    public LocationLineUp lineUp;

    public List<LocationBase> AllLocation;
    public List<LocationBuild> builds;
    public List<LocationBase> locations;

    [Space(10)]
    public AudioClip sfxBuildUnlock;

    private int currentBuildIndex = 0;
    private int nextCustomerIndex = 0;

    private int countCustomer;
    private MapData mapData;
    public UserMapData mapSave => UserData.map.GetMapData(UserData.map.currentMapIndex);

    private void Awake()
    {
        Debug.Log("Save Path: " + Application.persistentDataPath);

        GameObject.Find("GameData").GetComponent<GameData>().Init();
        GameObject.Find("UIGameManager").GetComponent<UIGameManager>().Init();

        currentMapIndex = UserData.map.currentMapIndex;

        UserData.PrepareMapSave(currentMapIndex);

        mapData = GameData.Instance.GetCurrentMapData(currentMapIndex);

        ShowUI();

        loadMapData();

        StartCoroutine(SpawnCustomer(6));

        CreatePlayer();
    }

    private void ShowUI()
    {
        UIGameManager.Instance.gameObject.SetActive(true);
        UIGame.Instance.HidePopups();
        Camera cam = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        if (cam != null)
        {
            Camera uiCamera = cam;

            if (UIGameManager.Instance.canvas != null)
            {
                UIGameManager.Instance.canvas.worldCamera = uiCamera;

                Debug.Log("worldCamera: " + UIGameManager.Instance.canvas.worldCamera);
            }
        }
    }

    private void CreatePlayer()
    {
        player.gameObject.SetActive(true);
        if (UserData.skin.GetOwnedSkins(SkinType.Set).Contains(UserData.skin.GetEquippedSkin(SkinType.Set)))
        {
            player.EquipSkinPlayer((SkinPlayerId)UserData.skin.GetEquippedSkin(SkinType.Set));
        }

        if (UserData.skin.GetOwnedSkins(SkinType.Glass).Contains(UserData.skin.GetEquippedSkin(SkinType.Glass)))
        {
            player.EquipSkinGlass((SkinGlassesId)UserData.skin.GetEquippedSkin(SkinType.Glass));
        }
        else
        {
            player.EquipSkinGlass(SkinGlassesId.None);
        }
    }

    #region Customer

    private IEnumerator SpawnCustomer(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (i >= customers.Count) break;

            var customer = customers[i];
            customer.gameObject.SetActive(true);
            customer.posIndex = i;

            nextCustomerIndex++;
            countCustomer++;

            yield return new WaitForSeconds(2f);
        }
    }

    public void DeplayToReActiveCustomer(AICustomer customer)
    {
        LeanTween.delayedCall(gameObject, 1f, () => customer.gameObject.SetActive(true));
    }

    #endregion

    public List<ItemOrder> GetOrders(int tableCount, bool isVip)
    {
        List<ItemOrder> tworks = new List<ItemOrder>();
        List<ItemId> items = new List<ItemId>();

        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].locationId == LocationId.Machine && locations[i].gameObject.activeInHierarchy)
            {
                items.Add(locations[i].GetProductId());
            }
        }

        int count = GameDataConstant.itemConfig.GetItemCountBuy(items.Count);
        int productPerCus = isVip ? UnityEngine.Random.Range(GameDataConstant.itemConfig.sVipMin, GameDataConstant.itemConfig.sVipMax) : UnityEngine.Random.Range(GameDataConstant.itemConfig.sMin, GameDataConstant.itemConfig.sMax);
        int total = tableCount * productPerCus;

        int quantity = (int)Mathf.Ceil((float)total / count);
        if (quantity < 1) quantity = 1;

        items = Helper.Shuffle(items);

        if (count > items.Count)
        {
            count = items.Count;
        }

        for (int i = 0; i < count; i++)
        {
            var twork = new ItemOrder
            {
                itemId = items[i],
                quantity = quantity
            };
            tworks.Add(twork);
        }

        return tworks;
    }

    public float GetPriceItem(ItemId itemId)
    {
        return GameDataConstant.itemConfig.GetItemPrice(itemId);
    }

    public Transform GetTransformCustomer(int index = -1)
    {
        if (index < 0) return transCustomers[UnityEngine.Random.Range(0, transCustomers.Count)];
        return transCustomers[index];
    }

    #region locationBuild

    private void loadMapData()
    {
        if (mapData == null) return;

        nextCustomerIndex = 0;
        countCustomer = 0;

        for (int i = 0; i < builds.Count; i++)
        {
            if (i < mapData.locations.Count)
            {
                builds[i].SetData(mapData.locations[i]);

                bool isUnlocked = mapSave.unlockedBuildIndexes.Contains(i);

                builds[i].gameObject.SetActive(false);
                locations[i].gameObject.SetActive(false);

                if (isUnlocked)
                {
                    locations[i].gameObject.SetActive(true);

                    if (locations[i].locationId == LocationId.Table)
                    {
                        LocationTable table = (LocationTable)locations[i];
                        int customerCount = mapSave.GetCustomerCount(i);
                        if (customerCount > 0)
                        {
                            int max = nextCustomerIndex + customerCount;
                            for (int j = nextCustomerIndex; j < max; j++)
                            {
                                if (j >= customers.Count) break;

                                var customer = customers[j];
                                customer.gameObject.SetActive(true);
                                customer.posIndex = j % transCustomers.Count;

                                nextCustomerIndex++;
                                countCustomer++;
                            }
                        }
                    }
                }
            }
        }

        int nextBuild = (mapSave.unlockedBuildIndexes?.Count > 0)
        ? mapSave.unlockedBuildIndexes.Max() + 1
        : 0;

        if (nextBuild < builds.Count && !mapSave.unlockedBuildIndexes.Contains(nextBuild))
        {
            builds[nextBuild].gameObject.SetActive(true);
        }
    }

    public void OnBuildCompleted(LocationBuild completedBuild)
    {
        AudioManager.Instance.audioSFX.PlayOneShot(sfxBuildUnlock);

        int buildIndex = builds.IndexOf(completedBuild);
        completedBuild.gameObject.SetActive(false);

        if (buildIndex < locations.Count)
        {
            locations[buildIndex].gameObject.SetActive(true);
            currentBuildIndex = buildIndex;
            BuildObject();
        }

        if (!mapSave.unlockedBuildIndexes.Contains(buildIndex))
        {
            mapSave.unlockedBuildIndexes.Add(buildIndex);
        }

        if (locations[buildIndex].locationId == LocationId.Table)
        {
            LocationTable table = (LocationTable)locations[buildIndex];
            int seats = table.transChairs?.Count ?? 0;
            mapSave.SetCustomerCount(buildIndex, seats);
        }

        UserData.Save();

        int nextBuildIndex = buildIndex + 1;
        if (nextBuildIndex < builds.Count)
        {
            builds[nextBuildIndex].gameObject.SetActive(true);
        }
    }

    private void BuildObject()
    {
        int mapId = UserData.map.currentMapIndex;
        var mapSave = UserData.map.allMapData[mapId];

        for (int i = 0; i < locations.Count; i++)
        {
            if (i == currentBuildIndex)
            {
                locations[i].gameObject.SetActive(true);

                if (locations[i].locationId == LocationId.Table)
                {
                    LocationTable table = (LocationTable)locations[i];
                    if (table.transChairs == null || table.transChairs.Count == 0) return;

                    int customerCount = table.transChairs.Count;

                    if (mapSave.data.GetCustomerCount(i) == 0)
                    {
                        mapSave.data.SetCustomerCount(i, customerCount);
                        UserData.Save();
                    }

                    int max = nextCustomerIndex + customerCount;
                    for (int j = nextCustomerIndex; j < max; j++)
                    {
                        if (j >= customers.Count) break;

                        var customer = customers[j];
                        if (!customer.gameObject.activeSelf)
                        {
                            customer.gameObject.SetActive(true);
                            customer.posIndex = j % transCustomers.Count;
                        }

                        nextCustomerIndex++;
                        countCustomer++;
                    }
                }
            }
        }
    }


    #endregion

    public List<LocationBase> GetLocationByType(LocationId locationId)
    {
        return AllLocation.Where(x => x.locationId == locationId && x.gameObject.activeSelf).ToList();
    }

    public LocationBase GetLocationNearesByItem(LocationId locationId, Vector3 pos)
    {
        var locations = GetLocationByType(locationId);
        float distance = 999999;

        LocationBase location = null;
        foreach (var lo in locations)
        {
            float dis = Vector3.Distance(lo.transform.position, pos);

            if (dis < distance)
            {
                location = lo;
                distance = dis;
            }
        }
        return location;
    }

    public List<AICustomer> TakeCustomerVips(int number)
    {
        List<AICustomer> temps = new List<AICustomer>();

        for (int i = 0; i < customerVips.Count; i++)
        {
            if (!customerVips[i].gameObject.activeSelf)
            {
                temps.Add((AICustomer)customerVips[i]);
                if (temps.Count >= number) break;
            }
        }

        if (temps.Count >= number) return temps;
        return null;
    }

    public void NextMap()
    {
        GameData.Instance.NextMap();
    }

    public bool SpendMoney(double money, bool reduceToZero = false)
    {
        double required = Math.Abs(money);

        if (UIGame.Instance.currentMoney < required)
        {
            if (reduceToZero && UIGame.Instance.currentMoney > 0)
            {
                UIGame.Instance.SubtractMoney(UIGame.Instance.currentMoney);
                return true;
            }
            //uiGamePlay.vkMoneyShake.ShakeCamera(0.025f, 0.2f);
            return false;
        }
        else
        {
            UIGame.Instance.SubtractMoney(required);
            return true;
        }
    }
}
