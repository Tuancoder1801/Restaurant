using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

public enum AICustomerState
{
    START,
    LINEUP,
    MOVETOTABLE,
    EATING,
    FINISH
}

public class AICustomer : AICharacter
{
    public Transform itemIndex;

    public GameObject goHappy;
    public GameObject[] goHappies;

    public List<ItemOrder> itemOrders = new List<ItemOrder>();
    public LocationBase locationTable;

    public AICustomerState state;

    public Transform trantarget;

    private float minDistance;
    private float lastDistance = 999;
    private float timeEating;

    private Action<Vector3, double> eatDoneCallback;
    private double eatMoney;

    private bool isVip;

    protected override void OnEnable()
    {
        base.OnEnable();

        isMoving = false;
        timeEating = 0;
        eatMoney = 0;
        goHappy.SetActive(false);

        var transform = GameManager.Instance.GetTransformCustomer(-1);
        this.transform.position = transform.position;

        StartFoodTour();
    }

    public void Update()
    {
        ChangeState();
    }

    private void ChangeState()
    {
        if (isMoving)
        {
            var d = Vector3.Distance(transform.position, targetPos);

            if (d < minDistance || (d < 4f && lastDistance == d))
            {
                StopMove();
            }
            else
            {
                lastDistance = d;
            }
        }
        else
        {
            switch (state)
            {
                case AICustomerState.START:
                    LineUp();
                    break;
                case AICustomerState.LINEUP:
                    LeanTween.cancel(gameObject);
                    LeanTween.rotate(gameObject, trantarget.eulerAngles, 0.3f);
                    break;
                case AICustomerState.MOVETOTABLE:
                    TableSit();
                    break;
                case AICustomerState.EATING:
                    if (timeEating > 0f)
                    {
                        timeEating -= Time.deltaTime;
                        if (timeEating <= 0f)
                        {
                            eatDoneCallback?.Invoke(transform.position, eatMoney);
                            eatDoneCallback = null;
                            eatMoney = 0;

                            PlayAnim(StaticValue.ANIM_TRIGGER_SIT);
                        }
                    }
                    break;
                case AICustomerState.FINISH:
                    gameObject.SetActive(false);
                    if (!isVip) GameManager.Instance.DeplayToReActiveCustomer(this);
                    break;
            }

        }
    }

    public void TakeItems(Transform transform)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        BaseItem item = transform.GetComponent<BaseItem>();

        if (item == null) return;

        sequence.Append(
        item.transform.DOMove(itemIndex.position, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(item.gameObject);
        })
        );
    }

    #region LinUp
    private void StartFoodTour()
    {
        state = AICustomerState.START;
        minDistance = 4;
        MoveToTarget(GameManager.Instance.lineUp.transform.position);
    }

    private void LineUp()
    {
        state = AICustomerState.LINEUP;
        trantarget = GameManager.Instance.lineUp.LineUp(this);
        minDistance = 0.1f;
        MoveToTarget(trantarget.position);
    }

    public void LineUpNext(Transform tranLineUp)
    {
        trantarget = tranLineUp;
        lastDistance = -99f;
        minDistance = 0.1f;
        MoveToTarget(trantarget.position);
    }

    #endregion

    #region Table

    public void TableInit(LocationBase table, Transform tranChair)
    {
        state = AICustomerState.MOVETOTABLE;
        locationTable = table;
        trantarget = tranChair;

        MoveToTarget(trantarget.position);
    }

    public void TableSit()
    {
        state = AICustomerState.EATING;

        gameObject.transform.position = trantarget.position;
        gameObject.transform.eulerAngles = trantarget.eulerAngles;

        PlayAnim(StaticValue.ANIM_TRIGGER_SIT);
        locationTable.CustomerSit();
    }

    public void TableEating(BaseItem item, double money, Action<Vector3, double> eatDone)
    {
        timeEating = 3f;
        eatMoney = money;
        eatDoneCallback = eatDone;

        gameObject.transform.eulerAngles = trantarget.eulerAngles;

        PlayAnim(StaticValue.ANIM_TRIGGER_EAT);
        TakeItems(item.transform);
    }

    public void TableEnd()
    {
        state = AICustomerState.FINISH;
        var transform = GameManager.Instance.GetTransformCustomer();
        MoveToTarget(transform.position);
    }

    public void TableEndVIP(bool isHappy = true)
    {
        isVip = true;

        goHappy.SetActive(true);
        goHappies[0].SetActive(isHappy);
        goHappies[1].SetActive(!isHappy);

        state = AICustomerState.FINISH;
        var transFrom = GameManager.Instance.GetTransformCustomer();
        MoveToTarget(transFrom.position);
    }

    public bool IsEating()
    {
        return timeEating > 0;
    }

    #endregion
}


