using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class AIWaiter : Character
{
    public Transform targetPos;
    public Transform waiterPos;

    public KitchenTable kitchenTable;
    public List<LocationTable> locationTables;

    private NavMeshAgent agent;
    private int currentTableIndex;
    private bool isMoving = false;
    private bool hasItemOrder = false;

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();

        currentTableIndex = 0;
    }

    private void Start()
    {
        //targetPos = waiterPos;
    }

    public override void Update()
    {
        base.Update();
        UpdateIdleHold();
        UpdateWalk();
        UpdateWalkHold();
        CheckAnim();
    }

    private void CheckAnim()
    {
        isMoving = targetPos != null;

        hasItemOrder = HasItemInTable();
    }

    #region Idle

    public override void UpdateIdle()
    {
        if (state != CharacterState.Idle) return;

        if (hasItemOrder)
        {
            if (HasItemInPlate())
            {
                ServeTable();
            }
            else
            {
                targetPos = waiterPos;
                ChangeState(CharacterState.Walk);
            }
        }
        else
        {
            targetPos = waiterPos;
            ChangeState(CharacterState.Walk);
        }

        if (isMoving)
        {
            ChangeState(CharacterState.Walk);
        }

        if (isHolding)
        {
            ChangeState(CharacterState.IdleHold);
        }

        //targetPos = waiterPos;
        //ChangeState(CharacterState.Walk);
    }

    public override void UpdateIdleHold()
    {
        if (state != CharacterState.IdleHold) return;

        if (!isHolding)
        {
            ChangeState(CharacterState.Idle);
        }

        if (currentItemNumber > 0)
        {
            GoToTable();
        }
    }

    #endregion

    #region Walk

    public override void UpdateWalk()
    {
        if (state != CharacterState.Walk) return;

        agent.isStopped = false;
        GetToTargetPos(targetPos, CharacterState.Idle);
    }

    public override void UpdateWalkHold()
    {
        if (state != CharacterState.WalkHold) return;

        agent.isStopped = false;
        GetToTargetPos(targetPos, CharacterState.IdleHold);
    }

    private void GetToTargetPos(Transform target, CharacterState characterState)
    {
        agent.SetDestination(target.position);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            targetPos = null;
            transform.position = target.position;
            transform.rotation = target.rotation;

            if (target == waiterPos)
            {
                ChangeState(CharacterState.Idle);
            }
            else
            {
                ChangeState(characterState);
            }
        }
    }

    #endregion

    private bool HasItemInPlate()
    {
        return kitchenTable != null && kitchenTable.plate.currentStackNumber > 0;
    }

    private bool HasItemInTable()
    {    
        foreach (var table in locationTables)
        {
            if (table.itemOrders != null)
            {
                foreach (var item in table.itemOrders)
                {
                    if (item.currentItemNumber < item.quantity)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void ServeTable()
    {
        while (currentTableIndex < locationTables.Count)
        {
            var table = locationTables[currentTableIndex];

            if (table.itemOrders != null)
            {
                foreach (var item in table.itemOrders)
                {
                    if (item.currentItemNumber < item.quantity)
                    {
                        GoToPlate(item.itemId);
                        return; // Dừng lại, chờ waiter lấy món và di chuyển đến bàn
                    }
                }
            }

            // Nếu bàn này không cần món, chuyển sang bàn tiếp theo
            currentTableIndex++;
        }

        // Nếu không có bàn nào cần món nữa, reset về bàn đầu tiên
        currentTableIndex = 0;
    }

    private void GoToPlate(ItemId itemId)
    {
        if (kitchenTable != null && itemId == kitchenTable.itemId)
        {
            targetPos = kitchenTable.waiterIndex;
            ChangeState(CharacterState.Walk);
        }
    }

    private void GoToTable()
    {
        var table = locationTables[currentTableIndex];

        if (table.itemOrders != null)
        {
            foreach (var item in table.itemOrders)
            {
                if (item.currentItemNumber < item.quantity && currentItemNumber > 0)
                {
                    targetPos = table.waiterIndex;
                    ChangeState(CharacterState.WalkHold);
                    return;
                }
            }
        }

        // Nếu bàn này đã đủ món, chuyển sang bàn tiếp theo
        currentTableIndex++;
        ServeTable();
    }

    //private void tableHasOrder()
    //{
    //    if (locationTables == null) return;

    //    if (locationTables[currentTable].itemOrders == null)
    //    {
    //        currentTable++;
    //    }

    //    if (currentTable >= locationTables.Count) currentTable = 0;

    //    Debug.Log("current table: " + currentTable);
    //}
}