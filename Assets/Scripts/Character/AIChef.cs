using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChef : Character
{
    public Transform targetPos;

    public Machine machine;

    private NavMeshAgent agent;

    public override void Awake()
    {   
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    public override void Update()
    {
        base.Update();
        UpdateWalk();
        UpdateCook();
    }

    public void ChangePos(Transform pos)
    {
        targetPos = pos;
    }

    #region Idle

    public override void UpdateIdle()
    {
        if (state != CharacterState.Idle) return;

        if(targetPos != null)
        {
            ChangeState(CharacterState.Walk);
        }

        if(machine != null && machine.isCooking)
        {
            ChangeState(CharacterState.Cook);
        }
    }

    #endregion

    #region Walk

    public override void UpdateWalk()
    {
        if (state != CharacterState.Walk) return;

        if(targetPos != null)
        {
            GetToTargetPos(targetPos);
        }
    }

    private void GetToTargetPos(Transform target)
    {
        agent.SetDestination(target.position);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            targetPos = null;
            transform.position = target.position;
            transform.rotation = target.rotation;

            ChangeState(CharacterState.Idle);
        }
    }

    #endregion

    #region Cook

    public override void UpdateCook()
    {
        if (state != CharacterState.Cook) return;

        if(machine != null && !machine.isCooking)
        {
            ChangeState(CharacterState.Idle);
        }
    }

    #endregion
}
