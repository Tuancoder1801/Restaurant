using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationLineUp : MonoBehaviour
{
    public Transform spawnPos;
    public Transform backPos;
    public Transform porterPos;

    public List<Transform> queuePos;
    public AICustomer customer;
    public List<AICustomer> customers;

    private bool takeLock;

    private void Awake()
    {
        //SpawnCustomer();
    }

    private void Start()
    {
        customers = new List<AICustomer>();
        takeLock = false;
    }

    private void Update()
    {
        //CheckEmptyTable();
    }

    #region Customer

    public Transform LineUp(AICustomer customer)
    {
        customers.Add(customer);
        return queuePos[customers.Count - 1];
    }

    public List<AICustomer> TakeCustomer(int number)
    {
        if (takeLock) return null;

        takeLock = true;

        List<AICustomer> temps = null;
        if(customers.Count >= number)
        {
            temps = new List<AICustomer>();

            for(int i = 0; i < number; i++)
            {
                var cs = customers[i];
                temps.Add(cs);
                customers.RemoveAt(i);
            }

            UpdateQueuePositions();
        }

        takeLock = false;
        return temps;
    }

    //private void SpawnCustomer()
    //{
    //    for (int i = 0; i < queuePos.Count; i++)
    //    {
    //        AICustomer newCustomer = Instantiate(customer, queuePos[i].position, queuePos[i].rotation);
    //        newCustomer.targetPos = queuePos[i];
    //        newCustomer.backPos = backPos;
    //        customerQueue.Add(newCustomer);
    //    }
    //}

    //private void AddCustomer(Transform targetPos)
    //{
    //    AICustomer newCustomer = Instantiate(customer, spawnPos.position, spawnPos.rotation);
    //    newCustomer.targetPos = targetPos;
    //    newCustomer.backPos = backPos;
    //    customerQueue.Add(newCustomer);
    //}

    private void UpdateQueuePositions()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            customers[i].ChangePos(queuePos[i]);
        }
    }

    //private void CheckEmptyTable()
    //{
    //    if (customerQueue == null || customerQueue.Count <= 0) return;

    //    List<LocationTable> availableTables = tables.Where(t => !t.isOccupied).ToList();

    //    while (availableTables.Count > 0 && customerQueue.Count > 0) // Kiểm tra cả bàn trống và khách đợi
    //    {
    //        LocationTable nextTable = availableTables[0];
    //        AICustomer firstCustomer = customerQueue[0];

    //        nextTable.isOccupied = true;
    //        firstCustomer.isInQueue = false;
    //        firstCustomer.ChangePos(nextTable.sittingPos);
    //        customerQueue.RemoveAt(0);
    //        availableTables.RemoveAt(0); // Cập nhật lại danh sách bàn trống
    //    }

    //    UpdateQueuePositions();

    //    // Kiểm tra nếu thiếu khách trong hàng đợi
    //    while (customerQueue.Count < queuePos.Count)
    //    {
    //        AddCustomer(queuePos[customerQueue.Count]);
    //    }
    //}

    public List<ItemId> GetAvailableItems()
    {
        List<ItemId> availableItems = new List<ItemId>();

        foreach (var kitchenTable in GameManager.Instance.kitchenTables)
        {
            availableItems.Add(kitchenTable.itemId);
        }

        return availableItems;
    }

    #endregion
}
