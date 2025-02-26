using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class AIPorter : Character
{
    public Transform targetPos;
    public Transform porterPos;

    public LocationMachine kitchenTable;
    public List<RawBin> rawBins;

    private NavMeshAgent agent;
    private bool isMoving = false;

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        targetPos = porterPos;
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

        hasItemInTray();
    }

    #region Idle

    public override void UpdateIdle()
    {
        if (state != CharacterState.Idle) return;

        if (hasItemInTray())
        {
            CheckItemInTray();
        }
        else
        {
            targetPos = porterPos;
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
            targetPos = kitchenTable.porterIndex;
            ChangeState(CharacterState.WalkHold);
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

            if (target == porterPos)
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

    private bool hasItemInTray()
    {
        if (kitchenTable != null)
        {
            foreach (var itemPos in kitchenTable.tray.itemsPosition)
            {
                if (itemPos.currentStackNumber < itemPos.maxStackNumber)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void CheckItemInTray()
    {
        if (kitchenTable != null)
        {
            foreach (var itemPos in kitchenTable.tray.itemsPosition)
            {
                if (itemPos.currentStackNumber < itemPos.maxStackNumber)
                {
                    FindItemInRawbins(itemPos.itemId);
                    return;
                }
            }
        }
    }

    private void FindItemInRawbins(ItemId itemId)
    {
        foreach (var rawbin in rawBins)
        {
            if (itemId == rawbin.baseItem.itemId)
            {
                targetPos = rawbin.pickIndex;
                ChangeState(CharacterState.Walk);
                return;
            }
        }
    }
}
