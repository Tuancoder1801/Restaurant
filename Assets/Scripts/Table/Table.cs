using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public UILocation uiLocation;

    public List<ItemOrder> itemOrders;

    private void Start()
    {
        uiLocation.gameObject.SetActive(true);

    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        AICustomer customer = other.GetComponent<AICustomer>();

        if(customer != null)
        {
            Debug.Log("cham");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
