using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTable : MonoBehaviour
{
    public UILocation uiLocation;
    public List<ItemOrder> itemOrders;
    public List<Transform> transforms;

    public AICustomer currentCustomer = null;

    private bool isPlayer = false;
    private Coroutine itemSpawnCoroutine;

    private void Start()
    {
        uiLocation.gameObject.SetActive(false);

    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        AICustomer customer = other.GetComponent<AICustomer>();

        if (customer != null)
        {
            currentCustomer = customer;
            itemOrders = customer.GetOrder();
            DisplayOrders();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        AICustomer customer = other.GetComponent<AICustomer>();
        Player player = other.GetComponent<Player>();

        if (customer != null && customer.state == CharacterState.Sit && customer == currentCustomer)
        {   
            itemOrders = customer.GetOrder();
            DisplayOrders();
        }

        if(player != null && !isPlayer)
        {
            isPlayer = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItemsCoroutine(player));
        }

    }

    private void OnTriggerExit(Collider other)
    {
        AICustomer customer = other.GetComponent<AICustomer>();
        Player player = other.GetComponent<Player>();

        if (customer != null && customer == currentCustomer)
        {
            currentCustomer = null;
            itemOrders.Clear();
            HideOrders();
            Debug.Log("Customer left the table.");
        }

        if (player != null)
        {
            isPlayer = false;
            if (itemSpawnCoroutine != null)
            {
                StopCoroutine(itemSpawnCoroutine);
            }
        }
    }

    private void DisplayOrders()
    {
        uiLocation.gameObject.SetActive(true);
        uiLocation.LoadProduct(itemOrders);
    }

    private void HideOrders()
    {
        uiLocation.gameObject.SetActive(false);
    }

    private IEnumerator SpawnItemsCoroutine(Player player)
    {
        while (isPlayer)
        {
            for (int i = 0; i < itemOrders.Count; i++)
            {
                if (itemOrders[i].currentItemNumber < itemOrders[i].quantity)
                {
                    player.ReleaseItems(itemOrders[i], transforms);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
