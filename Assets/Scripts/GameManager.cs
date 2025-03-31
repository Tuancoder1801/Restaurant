using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class GameManager : Singleton<GameManager>
{
    public int currentMapIndex = 0;

    [Header("Player")]
    [Space(20)]
    public Player player;
    public List<AICharacter> customers;
    public List<AICharacter> customerVips;

    public SmoothCamera smoothCamera;

    [Header("Transform")]
    [Space(20)]
    public List<Transform> transCustomers;
    public Transform playerPos;
    public List<Transform> transColectorWayPoints;

    public LocationLineUp lineUp;

    public List<LocationBase> AllLocation;

    public List<LocationBuild> builds;
    public List<LocationBase> locations;


    public List<ItemId> productItems = new List<ItemId>();


    private int currentBuildIndex = 0;
    private int countCustomer;
    private MapData mapData;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        mapData = GameData.Instance.GetCurrentMapData(currentMapIndex);

        countCustomer = 8;

        CreatePlayer();

        //StartCoroutine(SpawnCustomer(countCustomer));

        loadMapData();
    }

    private void CreatePlayer()
    {
        player.gameObject.SetActive(true);
        //smoothCamera.SetTarget(player.transform);
    }

    #region Customer

    private IEnumerator SpawnCustomer(int count)
    {
        for (int i = 0; i < count; i++)
        {
            customers[i].gameObject.SetActive(true);
            if (i < transCustomers.Count) customers[i].posIndex = i;
            else customers[i].posIndex = Random.Range(0, transCustomers.Count);
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

        int count = GameDataConstant.itemConfig.GetItemCountBuy(productItems.Count);
        int productPerCus = isVip ? UnityEngine.Random.Range(GameDataConstant.itemConfig.sVipMin, GameDataConstant.itemConfig.sVipMax) : UnityEngine.Random.Range(GameDataConstant.itemConfig.sMin, GameDataConstant.itemConfig.sMax);
        int total = tableCount * productPerCus;

        //if (uiGamePlay.IsBoosterCustomer()) total = total * 2;

        //if (TutorialController.instance != null && TutorialController.instance.currentTutorial <= 9) total = 4;

        int quantity = (int)Mathf.Ceil((float)total / count);
        if (quantity < 1) quantity = 1;

        productItems = Helper.Shuffle(productItems);
        for (int i = 0; i < count; i++)
        {
            var twork = new ItemOrder
            {
                itemId = productItems[i],
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
                    Debug.Log($"Found Table at index {i}");

                    LocationTable countChair = (LocationTable)locations[i];

                    if (countChair.transChairs == null || countChair.transChairs.Count == 0)
                    {
                        Debug.LogWarning("Table found but no chairs available.");
                        return;
                    }

                    int max = countCustomer + countChair.transChairs.Count;
                    Debug.Log("max: " + max);
                    for (int j = countCustomer; j < max; j++)
                    {
                        if (j < customers.Count)
                        {
                            customers[j].gameObject.SetActive(true);
                            if (i < transCustomers.Count) customers[i].posIndex = i;
                            else customers[i].posIndex = Random.Range(0, transCustomers.Count);
                            countCustomer++;
                        }
                        else
                            break;
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

            if(dis < distance)
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
}
