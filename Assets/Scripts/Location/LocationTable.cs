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

public class LocationTable : LocationBase
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
        customers = new List<AICustomer>();

        product.Init();
        uiLocation.gameObject.SetActive(false);

        StartCoroutine(IEWaitOrderCreate());
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
        Player player = other.GetComponent<Player>();
        AIWaiter waiter = other.GetComponent<AIWaiter>();

        if (player != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItemsCoroutine(player));
        }

        if (waiter != null && !isColliding)
        {
            isColliding = true;
            //itemSpawnCoroutine = StartCoroutine(SpawnItemsCoroutine(waiter));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        AIWaiter waiter = other.GetComponent<AIWaiter>();

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

    private IEnumerator SpawnItemsCoroutine(Character character)
    {
        while (isColliding)
        {
            //character.ReleaseItems(itemOrders, product);
            yield return new WaitForSeconds(0.3f);
        }
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
        for (int i = 0; i < itemOrders.Count; i++)
        {
            if (itemOrders[i].currentItemNumber >= itemOrders[i].quantity)
            {
                uiLocation.HideUIItem(itemOrders[i].itemId);
            }
        }
    }

    IEnumerator IEWaitOrderCreate()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            customers = GameManager.Instance.lineUp.TakeCustomer(transChairs.Count);

            if (customers != null) break;
            yield return new WaitForSeconds(2f);
        }

        countCustomer = 0;
        for (int i = 0; i < transChairs.Count; i++)
        {
            customers[i].TableInit(this, transChairs[i]);
        }
    }

    IEnumerator IEWaitOrderEating()
    {
        while (true)
        {
            if (product.IsHasItem())
            {
                if (!customers[nextCustomerEat].IsEating())
                {
                    var item = product.PopItem();
                    customers[nextCustomerEat].TableEating(item);
                }

                nextCustomerEat++;
                if (nextCustomerEat == customers.Count) nextCustomerEat = 0;
            }

            yield return new WaitForSeconds(0.8f);

            if (!uiLocation.itemContent.activeSelf && product.IsEmpty() && customers.All(x => !x.IsEating()))
            {
                break;
            }
        }

        yield return new WaitForSeconds(.5f);
        OrderEnd();
    }

    public void OrderStart()
    {
        itemOrders = GameManager.Instance.GetOrders();
        uiLocation.LoadProduct(itemOrders);
        uiLocation.gameObject.SetActive(true);

        nextCustomerEat = 0;
        StartCoroutine(IEWaitOrderEating());
    }

    public void OrderEnd()
    {
        customers.ForEach(x => x.TableEnd());
        uiLocation.gameObject.SetActive(false);
        itemOrders = null;

        StartCoroutine(IEWaitOrderCreate());
    }

    public void CustomerSit()
    {
        countCustomer++;

        if (countCustomer == customers.Count)
        {
            OrderStart();
        }
    }
}
