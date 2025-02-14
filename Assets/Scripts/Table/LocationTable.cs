using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TableType 
{
    ONE = 1,
    TWO = 2, 
    THREE = 3, 
    FOUR = 4,
}

public class LocationTable : MonoBehaviour
{   
    public TableType tableType;
    public List<Transform> transChairs;

    public UILocation uiLocation;
    public List<ItemOrder> itemOrders;
    public List<AICustomer> customers;

    public ItemPosition product;

    public Transform waiterIndex;

    private int nextCustomerEat;
    private int countCustomer;

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;

    private void Start()
    {   
        itemOrders = null;
        

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
        AIWaiter waiter = other.GetComponent<AIWaiter>();

        if (customer != null)
        {
            customer.locationTable = this;
            itemOrders = customer.itemOrders;
            DisplayOrders();
        }

        if (player != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItemsCoroutine(player));
        }

        if (waiter != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItemsCoroutine(waiter));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AICustomer customer = other.GetComponent<AICustomer>();

        Player player = other.GetComponent<Player>();
        AIWaiter waiter = other.GetComponent<AIWaiter>();

        if (customer != null )
        {
            uiLocation.gameObject.SetActive(false);
        }

        if (player != null)
        {
            isColliding = false;
            if (itemSpawnCoroutine != null)
            {
                StopCoroutine(itemSpawnCoroutine);
            }
        }

        if (waiter != null)
        {
            isColliding = false;
            if (itemSpawnCoroutine != null)
            {
                StopCoroutine(itemSpawnCoroutine);
            }
        }
    }

    #region uiLocation
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

    private IEnumerator SpawnItemsCoroutine(Character character)
    {
        while (isColliding)
        {
            character.ReleaseItems(itemOrders, transforms);
            yield return new WaitForSeconds(0.3f);
        }
    }
    #endregion

    IEnumerator IEWaitTaskEating()
    {
        while (true)
        {
            if(product.isHasItem())
            {   
               

                if (!customers[nextCustomerEat].IsEating())
                {
                    customers[nextCustomerEat].TableEating();
                }
            }
        }
    }

    public void OrderStart()
    {
        itemOrders = GameManager.Instance.GetOrders();
        uiLocation.LoadProduct(itemOrders);
        uiLocation.gameObject.SetActive(true);

        nextCustomerEat = 0;

    }

    public void CustomerSit()
    {
        countCustomer++;

        if(countCustomer == customers.Count)
    }
}
