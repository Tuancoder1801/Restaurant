using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTable : MonoBehaviour
{
    public UILocation uiLocation;
    public List<ItemOrder> itemOrders;
    public List<Transform> transforms;
    
    public bool isOccupied = false;
    public Transform destination;
    public Transform sittingPos;
    public Transform departurePos;

    private bool isPlayer = false;
    private Coroutine itemSpawnCoroutine;

    private void Start()
    {
        uiLocation.gameObject.SetActive(false);

    }

    private void Update()
    {
        if(itemOrders != null)
        {
            DisplayOrders();
            HideOrders();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        AICustomer customer = other.GetComponent<AICustomer>();
        Player player = other.GetComponent<Player>();

        if (customer != null && customer.state == CharacterState.Sit)
        {
            customer.locationTable = this;
            itemOrders = customer.GetOrder();
            DisplayOrders();
        }

        if (player != null && !isPlayer)
        {
            isPlayer = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItemsCoroutine(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AICustomer customer = other.GetComponent<AICustomer>();
        Player player = other.GetComponent<Player>();

        if (customer != null)
        {   
            uiLocation.gameObject.SetActive(false);
            isOccupied = false;
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

    public bool AllOrdersCompleted()
    {
        if (itemOrders != null)
        {
            foreach (var order in itemOrders)
            {
                if (order.currentItemNumber < order.quantity)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void DisplayOrders()
    {
        if (itemOrders != null)
        {
            uiLocation.gameObject.SetActive(true);
            uiLocation.LoadProduct(itemOrders);
        }
    }

    private void HideOrders()
    {
        for(int i = 0; i < itemOrders.Count; i++)
        {
            if (itemOrders[i].currentItemNumber >= itemOrders[i].quantity)
            {
                uiLocation.HideUIItem(itemOrders[i].itemId);
            }
        }
    }

    private IEnumerator SpawnItemsCoroutine(Player player)
    {
        while (isPlayer)
        {
            player.ReleaseItems(itemOrders, transforms);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
