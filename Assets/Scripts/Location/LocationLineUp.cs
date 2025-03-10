using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationLineUp : MonoBehaviour
{
    public List<Transform> queuePos;
    public List<AICustomer> customers;

    private bool takeLock;

    private void Start()
    {
        customers = new List<AICustomer>();
        takeLock = false;
    }

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

            for (int i = 0; i < customers.Count; i++)
            {
                customers[i].LineUpNext(queuePos[i]);
            }
        }

        takeLock = false;
        return temps;
    }
}
