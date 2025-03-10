using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SubLocationMoney : MonoBehaviour
{   
    public BaseItem itemMoney;
    public Transform tranMoney;
    public Vector3 start;
    public Vector3 range;
    public int xMax;
    public int yMax = 15;
    public int zMax;

    [Space(10)]
    public bool isSuitcase;

    private GameObject goPlayer;
    
    private IEnumerator ieWaitPaymentMoney;
    private IEnumerator ieWaitTakeMoney;

    private List<BaseItem> moneys;

    private double currentMoney = 0;
    private double moneyMax = 0;
    private int itemMax = 0;

    private void Start()
    {
        moneys = new List<BaseItem>();
        
        currentMoney = 0;
        itemMax = 0;
        moneyMax = itemMax * 1;
    }

    public void PaymentMoney(double money, Vector3 pos)
    {
        currentMoney += money;

        if(money > moneyMax) money = moneyMax;

        while(money > 0)
        {
            money--;

            var iMoney = Instantiate(itemMoney, pos + new Vector3(0f, 1f, 0f), Quaternion.identity);

            var mpos = GetMoneyPosition();

            if(mpos == StaticValue.vCompare)
            {
                DG.Tweening.Sequence sequence = DOTween.Sequence();
                sequence.Append(
                iMoney.transform.DOMove(mpos, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    Destroy(iMoney.gameObject);
                }));
            }
            else
            {
                DG.Tweening.Sequence sequence = DOTween.Sequence();
                sequence.Append(
                iMoney.transform.DOMove(mpos, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    iMoney.transform.SetParent(tranMoney);
                    iMoney.transform.localPosition = Vector3.zero;
                    iMoney.transform.localRotation = Quaternion.identity;
                    iMoney.transform.localScale = Vector3.one;
                }));
            }
        }

        ieWaitPaymentMoney = null;
        if (goPlayer != null) ;
    }

    private Vector3 GetMoneyPosition()
    {
        int count = moneys.Count;

        if(count >= itemMax) return StaticValue.vCompare;

        int x = (count/zMax)%xMax;
        int y = count / (xMax*zMax);
        int z = count%zMax;

        return new Vector3(start.x + x * range.x, start.y + y * range.y, start.z + z * range.z);
    }

    public void TakeMoney(Transform tran)
    {
        if(currentMoney <= 0) return;

        if(ieWaitTakeMoney != null) StopCoroutine(ieWaitTakeMoney);

        ieWaitTakeMoney = IEWaitTakeMoney(tran);
        StartCoroutine(ieWaitTakeMoney);
    }

    public IEnumerator IEWaitTakeMoney(Transform tran)
    {
        yield return new WaitForEndOfFrame();

        while(currentMoney > moneyMax)
        {
            for (int i = 0; i < zMax; i++)
            {
                if(currentMoney > moneyMax)
                {
                    
                }
            }
        }
    }
}
