using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public Transform spawnPos;
    public Transform backPos;
    public Transform porterPos;
    public List<Transform> queuePos;

    public List<LocationTable> tables;
    public List<KitchenTable> kitchenTables;

    public AICustomer customer;

    private List<AICustomer> customerQueue = new List<AICustomer>();

    private void Awake()
    {
        SpawnCustomer();
    }

    private void Update()
    {
        CheckEmptyTable();
    }

    #region Customer

    private void SpawnCustomer()
    {
        for (int i = 0; i < queuePos.Count; i++)
        {
            AICustomer newCustomer = Instantiate(customer, queuePos[i].position, queuePos[i].rotation);
            newCustomer.targetPos = queuePos[i];
            newCustomer.backPos = backPos;
            customerQueue.Add(newCustomer);
        }
    }

    private void AddCustomer(Transform targetPos)
    {
        AICustomer newCustomer = Instantiate(customer, spawnPos.position, spawnPos.rotation);
        newCustomer.targetPos = targetPos;
        newCustomer.backPos = backPos;
        customerQueue.Add(newCustomer);
    }

    private void UpdateQueuePositions()
    {
        for (int i = 0; i < customerQueue.Count; i++)
        {
            customerQueue[i].ChangePos(queuePos[i]);
        }
    }

    private void CheckEmptyTable()
    {
        Debug.Log("Checking for empty tables...");
        foreach (var table in tables)
        {
            Debug.Log($"Table {tables.IndexOf(table)} occupied: {table.isOccupied}");
        }

        if (customerQueue == null || customerQueue.Count <= 0) return;

        for (int i = 0; i < tables.Count; i++)
        {
            if (tables[i].isOccupied == false)
            {   
                tables[i].isOccupied = true;
                AICustomer firstCustomer = customerQueue[0];
                firstCustomer.isInQueue = false;
                firstCustomer.ChangePos(tables[i].sittingPos);
                customerQueue.Remove(firstCustomer);
                UpdateQueuePositions();
                //AddCustomer(queuePos[customerQueue.Count]);
                return;
            }
        }

        if (customerQueue.Count < queuePos.Count)
        {
            AddCustomer(queuePos[customerQueue.Count]);
        }

        return;
    }

    private bool HasEmptyTable()
    {
        foreach (var table in tables)
        {
            if (!table.isOccupied) // Kiểm tra bàn trống
            {
                return true;
            }
        }
        return false;
    }

    public List<ItemId> GetAvailableItems()
    {
        List<ItemId> availableItems = new List<ItemId>();

        foreach (var kitchenTable in kitchenTables)
        {
            availableItems.Add(kitchenTable.itemId);
        }

        return availableItems;
    }

    #endregion
}
