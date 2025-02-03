using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPorter : Character
{
    public Transform targetPos;
    public Transform porterPos;

    private NavMeshAgent agent;

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
        UpdateWalk();
    }

    #region Idle

    public override void UpdateIdle()
    {
        if (state != CharacterState.Idle) return;

        if(targetPos != null)
        {
            ChangeState(CharacterState.Walk);
        }
    }

    public override void UpdateIdleHold()
    {
        if (state != CharacterState.IdleHold) return;
    }

    #endregion

    #region Walk

    public override void UpdateWalk()
    {
        if (state != CharacterState.Walk) return;

        if(targetPos != null)
        {   
            agent.isStopped = false;
            GetToTargetPos(targetPos, CharacterState.Idle);
        }
    }

    public override void UpdateWalkHold()
    {
        if (state != CharacterState.WalkHold) return;
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

            ChangeState(characterState);
        }
    }

    #endregion
}
