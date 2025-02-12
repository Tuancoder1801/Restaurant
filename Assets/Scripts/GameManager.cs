using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player;
    public AIChef chef;
    public AIPorter porter;
    public AIWaiter waiter;

    public Location location;

    public SmoothCamera smoothCamera;
    public Transform playerPos;
    public Transform spawnPos;
    public Transform porterPos;
    public Transform waiterPos;

    private LocationLineUp lineUp;

    public List<LocationTable> tables;
    public List<KitchenTable> kitchenTables;

    private void Awake()
    {
        GameDataConstant.Load();

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

    #region Chef

    private void SpawnChef()
    {
        for (int i = 0; i < location.kitchenTables.Count; i++)
        {
            AIChef aIChef = Instantiate(chef, spawnPos.position, spawnPos.rotation);
            aIChef.targetPos = location.kitchenTables[i].chefIndex;
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
}
