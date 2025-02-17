using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Player")]
    [Space(20)]
    public Player player;
    public List<AICharacter> customers;
    public AIChef chef;
    public AIPorter porter;
    public AIWaiter waiter;

    public Location location;

    public SmoothCamera smoothCamera;

    [Header("Transform")]
    [Space(20)]
    public List<Transform> transCustomers;
    public Transform playerPos;
    public Transform spawnPos;
    public Transform porterPos;
    public Transform waiterPos;

    public LocationLineUp lineUp;


    private void Awake()
    {
        GameDataConstant.Load();

        Init();
    }

    private void Update()
    {
        StartCoroutine(SpawnCustomer());
    }

    private void Init()
    {
        CreatePlayer();

        SpawnChef();
        SpawnPorter();
        SpawnWaiter();
    }

    private void CreatePlayer()
    {
        Player newPlayer = Instantiate(player, playerPos.position, playerPos.rotation);
        smoothCamera.SetTarget(newPlayer.transform);
    }

    #region Customer

    private IEnumerator SpawnCustomer()
    {
        int numQueue = Mathf.Min(lineUp.queuePos.Count, customers.Count); // Chỉ lấy tối đa số lượng hàng đợi

        for (int i = 0; i < numQueue; i++)
        {
            customers[i].gameObject.SetActive(true);
            if (i < transCustomers.Count) customers[i].posIndex = i;
            else customers[i].posIndex = Random.Range(0, transCustomers.Count);
            yield return new WaitForSeconds(2f);
        }
    }

    #endregion

    #region Chef

    private void SpawnChef()
    {
        for (int i = 0; i < location.kitchenTables.Count; i++)
        {        
            chef.gameObject.SetActive(true);
            chef.kitchen = location.kitchenTables[i];
        }
    }

    #endregion

    #region Porter

    private void SpawnPorter()
    {
        for (int i = 0; i < location.kitchenTables.Count; i++)
        {
            AIPorter aIPorter = Instantiate(porter, spawnPos.position, spawnPos.rotation);
            aIPorter.porterPos = porterPos;
            aIPorter.kitchenTable = location.kitchenTables[i];

            aIPorter.rawBins = GetRelevantRawbins(location.kitchenTables[i], location.rawBins);
        }
    }

    private List<RawBin> GetRelevantRawbins(KitchenTable kitchenTable, List<RawBin> allRawbins)
    {
        List<RawBin> relevantRawbins = new List<RawBin>();

        foreach (var itemPos in kitchenTable.tray.itemsPosition) // Lấy danh sách itemId từ Tray
        {
            foreach (var rawbin in allRawbins) // Duyệt qua danh sách Rawbin trong Location
            {
                if (!relevantRawbins.Contains(rawbin))
                {
                    relevantRawbins.Add(rawbin);
                }
            }
        }
        return relevantRawbins;
    }

    #endregion

    #region Waiter

    private void SpawnWaiter()
    {
        for (int i = 0; i < location.kitchenTables.Count; i++)
        {
            AIWaiter aiWaiter = Instantiate(waiter, spawnPos.position, spawnPos.rotation);
            aiWaiter.waiterPos = waiterPos;
            aiWaiter.kitchenTable = location.kitchenTables[i];
            aiWaiter.locationTables = location.locationTables;
        }
    }


    #endregion

    public List<ItemOrder> GetOrders()
    {
        List<ItemId> items = new List<ItemId>();
        List<ItemOrder> orders = new List<ItemOrder>();

        for (int i = 0; i < location.kitchenTables.Count; i++)
        {
            items.Add(location.kitchenTables[i].itemId);
        }

        int orderCount = Mathf.Min(UnityEngine.Random.Range(1, 2), items.Count);

        for (int i = 0; i < orderCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, items.Count);
            ItemId selectedItemId = items[randomIndex];

            ItemOrder newOrder = new ItemOrder
            {
                itemId = selectedItemId,
                quantity = UnityEngine.Random.Range(1, 3),
                currentItemNumber = 0,
            };

            orders.Add(newOrder);
            items.RemoveAt(randomIndex);
        }

        return orders;
    }

    public Transform GetTransformCustomer(int index = -1)
    {
        if (index < 0) return transCustomers[UnityEngine.Random.Range(0, transCustomers.Count)];
        return transCustomers[index];
    }
}
