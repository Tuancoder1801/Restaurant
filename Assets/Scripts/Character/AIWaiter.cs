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
    private LocationTable currentTable;
    private bool isMoving = false;
    private bool hasItem = false;

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        targetPos = waiterPos;
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

        //hasItem = HasItemInPlate() && HasItemInTable();
    }

    #region Idle

    public override void UpdateIdle()
    {
        if (state != CharacterState.Idle) return;

        //if (hasItem)
        //{
        //    //ServeTable();
            
        //}

        for(int i = 0; i < locationTables.Count; i++)
        {
            if (tableHasOrder(locationTables[i]))
            {
                currentTable = locationTables[i];
                ServeTable(currentTable);

                return;
            }
        }
     
        if (isMoving)
        {
            ChangeState(CharacterState.Walk);
        }

        if (isHolding)
        {
            ChangeState(CharacterState.IdleHold);
        }

        targetPos = waiterPos;
        ChangeState(CharacterState.Walk);
    }

    public override void UpdateIdleHold()
    {
        if (state != CharacterState.IdleHold) return;

        if (!isHolding)
        {
            ChangeState(CharacterState.Idle);
        }

        if (currentItemNumber > 0 && HasItemInTable())
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
        return kitchenTable.plate.currentStackNumber > 0;
    }

    private bool HasItemInTable()
    {
        if (locationTables != null)
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
        }
        return false;
    }

    private void ServeTable(LocationTable table)
    {
        if (table != null)
        {
            
        }
    }

    private void GoToPlate(ItemId itemId)
    {
        if (kitchenTable != null)
        {
            if (itemId == kitchenTable.itemId)
            {
                targetPos = kitchenTable.waiterIndex;
                ChangeState(CharacterState.Walk);
                return;
            }
        }
    }

    private void GoToTable()
    {
        if (locationTables != null)
        {
            foreach (var table in locationTables)
            {
                if (tableHasOrder(table)) // Nếu bàn này vẫn cần món
                {
                    targetPos = table.waiterIndex;
                    ChangeState(CharacterState.WalkHold);
                    return; // Phục vụ bàn này trước, sau đó mới kiểm tra bàn khác
                }
            }
        }
    }

    private bool tableHasOrder(LocationTable table)
    {
        foreach (var item in table.itemOrders)
        {
            if (item.currentItemNumber < item.quantity)
            {
                return true; // Nếu bàn vẫn cần món, waiter sẽ phục vụ
            }
        }
        return false;
    }
}

