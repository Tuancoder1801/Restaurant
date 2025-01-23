using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AICustomerManager : MonoBehaviour
{
    public Transform spawnPos;
    public Transform backPos;
    public List<Transform> queuePos;

    public List<LocationTable> tables;
    public AICustomer customer;
    public float spawnInterval = 3f;

    public List<AICustomer> customerQueue = new List<AICustomer>();

    private void Start()
    {
        SpawnCustomer();
    }

    private void Update()
    {
        if (HasEmptyTable())
        {
            CheckEmptyTable();
        }
    }

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
        if (customerQueue == null || customerQueue.Count <= 0) return;

        AICustomer firstCustomer = customerQueue[0];
       
        for (int i = 0; i < tables.Count; i++)
        {
            if (tables[i].isOccupied == false)
            {
                tables[i].isOccupied = true;
                firstCustomer.isInQueue = false;
                firstCustomer.ChangePos(tables[i].sittingPos);
                customerQueue.Remove(firstCustomer);
                UpdateQueuePositions();
                AddCustomer(queuePos[queuePos.Count - 1]);
            }
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
}
