using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class LocationTableVip : LocationBase
{
    public ItemPosition product;
    public List<Transform> tranChairs;

    public SubLocationMoney locationMoney;

    public UILocation uiLocation;

    public GameObject goStatusContain;
    public GameObject[] goStatus;
    public Text txtTime;

    public List<ItemOrder> itemOrders;
    public List<AICustomer> customers;

    private int nextCustomerEat;
    private int countCustomer;

    public float money;
    public bool isJustUnlock = false;

    public float timeEatCurrent;
    public float timeWait;
    private TimeSpan timeSpan;
    private string strTimeFormat = "{0:00}m {1:00}s";
    private GameObject goTable;
    private float bonus = 1f;

    private void Start()
    {
        itemOrders = null;
        customers = new List<AICustomer>();

        product.Init();
        uiLocation.transform.localScale = new Vector3(0.02f, 0.02f, 1f);

        StartCoroutine(IEWaitTaskCreate(isJustUnlock ? -1f : 300f));
        if (isJustUnlock) isJustUnlock = false;
    }

    public override BaseItem PopItem()
    {
        return product.PopItem();
    }

    public override void PushItem(BaseItem item)
    {
        if (itemOrders != null)
        {
            var task = itemOrders.FirstOrDefault(x => x.itemId == item.itemId);
            if (task != null && task.currentItemNumber < task.quantity)
            {
                task.currentItemNumber++;
                uiLocation.SetNumber(task.itemId, task.currentItemNumber, task.quantity);
                if (task.currentItemNumber >= task.quantity)
                {
                    uiLocation.HideUIItem(task.itemId);
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
        if (timeEatCurrent <= 0) return null;

        List<ItemId> needItems = new List<ItemId>();
        if (itemOrders != null && itemOrders.Count > 0)
        {
            foreach (var task in itemOrders)
            {
                if (task.currentItemNumber < task.quantity)
                {
                    needItems.Add(task.itemId);
                }
            }
            return needItems;
        }
        return null;
    }

    public override List<double> GetMoneys()
    {
        return locationMoney.GetMoney();
    }

    public override void LoadMoneys(List<double> moneys)
    {
        locationMoney.LoadMoney(moneys);
    }

    IEnumerator IEWaitTaskCreate(float timew)
    {
        uiLocation.itemContent.SetActive(false);
        //goHappy.SetActive(false);
        goStatusContain.SetActive(true);
        goStatus[0].SetActive(true);
        goStatus[1].SetActive(false);

        // count time
        timeWait = timew;
        ShowTime(timeWait);

        float timeTemp = 1f;
        while (timeWait > 0)
        {
            yield return new WaitForEndOfFrame();
            timeTemp -= Time.deltaTime;
            if (timeTemp <= 0)
            {
                timeTemp = 1f;
                timeWait -= 1f;
                ShowTime(timeWait);
            }
        }

        // ui
        goStatusContain.SetActive(false);

        yield return new WaitForEndOfFrame();
        while (true)
        {
            customers = GameManager.Instance.TakeCustomerVips(tranChairs.Count);

            if (customers != null) break;
            yield return new WaitForSeconds(2f);
        }

        GameManager.Instance.smoothCamera.ZoomInToTarget(new List<Transform> { customers[0].transform });
        customers.ForEach(x => x.gameObject.SetActive(true));
        yield return new WaitForEndOfFrame();

        countCustomer = 0;
        for (int i = 0; i < tranChairs.Count; i++)
        {
            customers[i].TableInit(this, tranChairs[i]);
        }
    }

    IEnumerator IEWaitTaskEating()
    {
        //goHappy.SetActive(false);
        goStatusContain.SetActive(true);
        goStatus[0].SetActive(false);
        goStatus[1].SetActive(true);

        if (timeEatCurrent <= 0) timeEatCurrent = 300f;
        float timeTemp = 1f;
        float timeCheck = 0.8f;

        LeanTween.dispatchEvent(2);

        while (timeEatCurrent > 0 || product.IsHasItem())
        {
            yield return new WaitForEndOfFrame();
            if (timeEatCurrent > 0)
            {
                timeTemp -= Time.deltaTime;
                if (timeTemp <= 0)
                {
                    timeTemp = 1f;
                    timeEatCurrent -= 1f;
                    ShowTime(timeEatCurrent);
                }
            }

            timeCheck -= Time.deltaTime;
            if (timeCheck <= 0)
            {
                timeCheck = 0.8f;

                if (product.IsHasItem())
                {
                    if (!customers[nextCustomerEat].IsEating())
                    {
                        var item = product.PopItem();
                        customers[nextCustomerEat].TableEating(item, 0, (pos, moneyEat) => { });
                    }

                    // payment money
                    // money += GamePlayController.Instance._itemConfig.GetItemPrice(item._itemType);
                    nextCustomerEat++;
                    if (nextCustomerEat == customers.Count) nextCustomerEat = 0;
                }

                if (!uiLocation.itemContent.activeSelf && product.IsEmpty() && customers.All(x => !x.IsEating()))
                {
                    break;
                }
            }
        }

        goStatusContain.SetActive(false);

        if (!uiLocation.itemContent.activeSelf)
        {
            //goHappy.SetActive(true);

            // fake UI
            yield return new WaitForSeconds(1.5f);
            TaskEnd(true);
        }
        else
        {
            // chưa hoàn thành task, khách bỏ đi và không trả tiền
            yield return new WaitForSeconds(1f);
            TaskEnd(false);
        }
    }

    public void TaskStart()
    {
        // task
        if (itemOrders == null)
        {
            itemOrders = GameManager.Instance.GetOrders(tranChairs.Count, true);
            money = 0;
            itemOrders.ForEach(x => {
                money += GameManager.Instance.GetPriceItem(x.itemId) * x.quantity;
            });
        }
        uiLocation.LoadProduct(itemOrders);

        nextCustomerEat = 0;
        StartCoroutine(IEWaitTaskEating());
    }

    public void TaskEnd(bool isHappy)
    {
        LeanTween.dispatchEvent(3);

        customers.ForEach(x => x.TableEndVIP(isHappy));
        itemOrders = null;
        timeEatCurrent = -1;

        if (isHappy)
        {
            locationMoney.PaymentMoney((int)(money * bonus), customers[0].transform.position);
        }
        money = 0;

        StartCoroutine(IEWaitTaskCreate(300f));
    }

    public override void CustomerSit()
    {
        countCustomer++;
        if (countCustomer == customers.Count)
        {
            GameManager.Instance.smoothCamera.ZoomInToTarget(new List<Transform> { transform });
            TaskStart();
        }
    }

    private void ShowTime(float timeShow)
    {
        if (timeShow <= 0) timeShow = 0;
        timeSpan = TimeSpan.FromSeconds(timeShow);
        txtTime.text = string.Format(strTimeFormat, timeSpan.Minutes, timeSpan.Seconds);
    }
}
 