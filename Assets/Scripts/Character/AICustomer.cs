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

    public List<ItemOrder> itemOrders = new List<ItemOrder>();
    public LocationTable locationTable;

    public AICustomerState state;

    private Transform trantarget;
    private LocationTable table;

    private float minDistance;
    private float lastDistance = 999;
    private float timeEating;

    protected override void OnEnable()
    {
        base.OnEnable();

        isMoving = false;
        timeEating = 0;

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
                    LeanTween.rotate(gameObject, trantarget.eulerAngles, 0.3f);
                    break;
                case AICustomerState.MOVETOTABLE:
                    break;
                case AICustomerState.EATING:
                    break;
                case AICustomerState.FINISH:
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

    public void TableInit(LocationTable table, Transform tranChair)
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

        Anim(StaticValue.ANIM_TRIGGER_SIT);
    }

    public void TableEating(BaseItem item)
    {
        timeEating = 3f;
        gameObject.transform.eulerAngles = trantarget.eulerAngles;

        Anim(StaticValue.ANIM_TRIGGER_EAT);

    }

    public void TableEnd()
    {
        state = AICustomerState.FINISH;

        MoveToTarget(transform.position);
    }

    public bool IsEating()
    {
        return timeEating > 0;
    }

    #endregion
}


