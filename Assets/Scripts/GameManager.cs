using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

    private float boxRange = -99;
    private int currentBuildIndex = 0;
    private int nextCustomerIndex = 0;

    private int countCustomer;
    private MapData mapData;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        mapData = GameData.Instance.GetCurrentMapData(currentMapIndex);

        CreatePlayer();

        loadMapData();

        StartCoroutine(SpawnCustomer(6));
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

        for (int i = 0; i < builds.Count; i++)
        {
            if (i < mapData.locations.Count)
            {
                builds[i].SetData(mapData.locations[i]);
                builds[i].gameObject.SetActive(false);
            }
        }

        if (builds.Count > 0)
        {
            builds[0].gameObject.SetActive(true);
        }
    }

    public void OnBuildCompleted(LocationBuild completedBuild)
    {
        completedBuild.gameObject.SetActive(false);
        BuildObject();
        currentBuildIndex++;
        if (currentBuildIndex < builds.Count)
        {
            builds[currentBuildIndex].gameObject.SetActive(true);
        }
    }

    private void BuildObject()
    {
        for (int i = 0; i < locations.Count; i++)
        {
            if (i == currentBuildIndex)
            {
                locations[i].gameObject.SetActive(true);

                if (locations[i].locationId == LocationId.Table)
                {
                    LocationTable countChair = (LocationTable)locations[i];

                    if (countChair.transChairs == null || countChair.transChairs.Count == 0) return;

                    int seats = countChair.transChairs.Count;
                    int max = nextCustomerIndex + seats;

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
        Debug.Log($"[SpendMoney] Try spending {required}, current = {UIGame.Instance.currentMoney}");

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
