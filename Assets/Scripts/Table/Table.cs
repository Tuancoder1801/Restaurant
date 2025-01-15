using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public UILocation uiLocation;

    private AICustomer customer;

    private void Start()
    {
        uiLocation.gameObject.SetActive(true); 
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        customer = GetComponent<AICustomer>();

        if(customer != null)
        {

        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
