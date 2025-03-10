using DG.Tweening;
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

    public SubLocationMoney locationMoney;

    public UILocation uiLocation;
    public List<ItemOrder> itemOrders;
    public List<AICustomer> customers;

    public ItemPosition product;

    private int nextCustomerEat;
    private int countCustomer;

    private void Start()
    {
        itemOrders = null;
        customers = new List<AICustomer>();

        product.Init();
        uiLocation.gameObject.SetActive(false);

        StartCoroutine(IEWaitOrderCreate());
    }

    public override BaseItem PopItem()
    {
        return product.PopItem();
    }

    public override void PushItem(BaseItem item)
    {
        if(itemOrders != null)
        {
            var order = itemOrders.FirstOrDefault(x => x.itemId == item.itemId);

            if(order != null && order.currentItemNumber < order.quantity)
            {
                order.currentItemNumber++;
                uiLocation.SetNumber(order.itemId, order.currentItemNumber, order.quantity);

                if(order.currentItemNumber >= order.quantity)
                {
                    uiLocation.HideUIItem(order.itemId);
                }
            }
        }

        int index = product.GetIndexEmpty();
        product.PushItem(item, index);

        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Append(
        item.transform.DOJump(product.itemPositions[index].position, 1f, 1, 0.2f).OnComplete(() =>
        {
            item.transform.SetParent(product.itemPositions[index]);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;
        }));
    }

    public override List<ItemId> GetNeedItems()
    {
        List<ItemId> needItems = new List<ItemId>();
        if(itemOrders != null && itemOrders.Count > 0)
        {
            foreach (var order in itemOrders)
            {
                if(order.currentItemNumber < order.quantity)
                {
                    needItems.Add(order.itemId);
                }
            }
            return needItems;
        }
        return null;
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
                    customers[nextCustomerEat].TableEating(item, 2f, (pos, moneyEat) =>
                    {
                        locationMoney.PaymentMoney(moneyEat, pos + new Vector3(0f, 0.8f, 0f));
                    });
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
        itemOrders = GameManager.Instance.GetOrders(1);
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

    public bool IsNeedItem(ItemId itemId)
    {
        if (itemOrders != null && itemOrders.Count > 0)
        {
            foreach (var order in itemOrders)
            {
                if(order.itemId == itemId && order.currentItemNumber < order.quantity)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
