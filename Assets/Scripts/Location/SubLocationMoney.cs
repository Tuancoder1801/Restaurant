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
    private GameObject goCollector;

    private IEnumerator ieWaitPaymentMoney;
    private IEnumerator ieWaitTakeMoney;

    private List<BaseItem> moneys;

    public double currentMoney = 0;
    private double moneyMax = 0;
    private int itemMax = 0;

    private void Start()
    {
        moneys = new List<BaseItem>();

        currentMoney = 0;
        itemMax = (xMax * yMax * zMax);
        moneyMax = itemMax * 1;
    }

    protected void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(StaticValue.CHARACTER_NAME_TAG))
        {
            goPlayer = other.gameObject;
            TakeMoney(goPlayer.transform);
        }
        else if (other.CompareTag(StaticValue.COLLECCTOR_NAME_TAG))
        {
            goCollector = other.gameObject;
            if (goPlayer == null) TakeMoney(goCollector.transform);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StaticValue.CHARACTER_NAME_TAG))
        {
            goPlayer = null;

            CancelTakeMoney();
        }
        else if (other.CompareTag(StaticValue.COLLECCTOR_NAME_TAG))
        {
            goCollector = null;
            if (goPlayer == null) CancelTakeMoney();
        }
    }

    public void PaymentMoney(double money, Vector3 pos)
    {
        currentMoney += money;

        if (money > moneyMax) money = moneyMax;

        while (money > 0)
        {
            money -= 1;

            var iMoney = Instantiate(itemMoney, pos + new Vector3(0f, 1f, 0f), Quaternion.identity);

            var mpos = GetMoneyPosition();

            if (mpos == StaticValue.vCompare)
            {
                iMoney.MoveNormal(tranMoney, pos + new Vector3(0f, 1f, 0f), mpos, new Vector3(0f, UnityEngine.Random.Range(-12f, 12f), 0f), 0.1f, () =>
                {
                    Destroy(iMoney.gameObject);
                });
            }
            else
            {
                moneys.Add(iMoney);

                iMoney.MoveNormal(tranMoney, pos + new Vector3(0f, 1f, 0f), mpos, new Vector3(0f, UnityEngine.Random.Range(-12f, 12f), 0f), 0.1f, () =>
                {
                    // nothing
                });
            }
        }

        ieWaitPaymentMoney = null;
        if (goPlayer != null)
        {
            TakeMoney(goPlayer.transform); // ưu tiên player
        }
        else if (goCollector != null)
        {
            TakeMoney(goCollector.transform);
        }
    }

    private Vector3 GetMoneyPosition()
    {
        int count = moneys.Count;

        if (count >= itemMax) return StaticValue.vCompare;

        int x = (count / zMax) % xMax;
        int y = count / (xMax * zMax);
        int z = count % zMax;

        return new Vector3(start.x + x * range.x, start.y + y * range.y, start.z + z * range.z);
    }

    public void TakeMoney(Transform tran)
    {
        if (currentMoney <= 0) return;

        if (ieWaitTakeMoney != null) StopCoroutine(ieWaitTakeMoney);

        ieWaitTakeMoney = IEWaitTakeMoney(tran);
        StartCoroutine(ieWaitTakeMoney);
    }

    public IEnumerator IEWaitTakeMoney(Transform tran)
    {
        yield return new WaitForEndOfFrame();

        while (currentMoney > moneyMax)
        {
            for (int i = 0; i < zMax; i++)
            {
                if (currentMoney > moneyMax)
                {
                    var iMoney = Instantiate(itemMoney, tranMoney.position, tranMoney.rotation);
                    iMoney.transform.localPosition = Vector3.zero;
                    iMoney.transform.localScale = Vector3.one;
                    iMoney.transform.localEulerAngles = Vector3.zero;

                    DG.Tweening.Sequence seq = DOTween.Sequence();
                    seq.Append(iMoney.transform.DOMoveY(iMoney.transform.position.y + 2f, 0.1f).SetEase(Ease.OutQuad));
                    seq.Append(iMoney.transform.DOMoveY(tran.position.y, 0.05f).SetEase(Ease.InQuad));
                    //seq.Join(iMoney.transform.DOMoveX(tran.position.x, 0.3f));
                    seq.Join(iMoney.transform.DOMoveZ(tran.position.z, 0.05f));

                    seq.OnComplete(() =>
                    {
                        Destroy(iMoney.gameObject);
                        UIGame.Instance.AddMoney(1);
                    });

                    currentMoney -= 1;
                }
                else
                {
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }

        while (moneys.Count > 0)
        {
            for (int i = 0; i < zMax; i++)
            {
                if (moneys.Count <= 0) break;
                var iMoney = moneys[moneys.Count - 1];
                moneys.Remove(iMoney);

                DG.Tweening.Sequence seq = DOTween.Sequence();
                seq.Append(iMoney.transform.DOMoveY(iMoney.transform.position.y + 2f, 0.1f).SetEase(Ease.OutQuad));
                seq.Append(iMoney.transform.DOMoveY(tran.position.y, 0.05f).SetEase(Ease.InQuad));
                //seq.Join(iMoney.transform.DOMoveX(tran.position.x, 0.3f));
                seq.Join(iMoney.transform.DOMoveZ(tran.position.z, 0.05f));

                seq.OnComplete(() =>
                {
                    Destroy(iMoney.gameObject);
                    UIGame.Instance.AddMoney(1);
                });
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }

        currentMoney = 0;

        ieWaitTakeMoney = null;
    }

    public void CancelTakeMoney()
    {
        if (ieWaitTakeMoney != null) StopCoroutine(ieWaitTakeMoney);
    }

    public List<double> GetMoney()
    {
        return new List<double> { currentMoney };
    }

    public void LoadMoney(List<double> lMoneys)
    {
        if (moneys == null || moneys.Count <= 0) return;

        double money = lMoneys[0];
        currentMoney += money;

        while (money > 0)
        {
            // reduce
            money -= 1;

            // pos
            var mpos = GetMoneyPosition();
            if (mpos == StaticValue.vCompare)
            {
                break;
            }
            else
            {
                //var iMoney = ItemController.Instance.BorrowItem(ItemType.MONEY);
                //moneys.Add(iMoney);

                //iMoney.transform.SetParent(tranMoney);
                //iMoney.transform.localPosition = mpos;
                //iMoney.transform.localScale = Vector3.one;
                //iMoney.transform.localEulerAngles = new Vector3(0f, UnityEngine.Random.Range(-12f, 12f), 0f);
                //iMoney.gameObject.SetActive(true);
            }
        }
    }
}
