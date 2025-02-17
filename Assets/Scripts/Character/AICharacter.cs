using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacter : MonoBehaviour
{
    public int posIndex;

    protected NavMeshAgent agent;
    protected Animator animator;

    public Vector3 targetPos;

    protected bool isMoving;

    protected virtual void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void ResetAllTriggers()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }

    public void Anim(string value)
    {
        ResetAllTriggers();
        animator.SetTrigger(value);
    }

    protected void MoveToTarget(Vector3 pos)
    {
        targetPos = pos;

        isMoving = true;
        agent.enabled = true;
        agent.SetDestination(targetPos);
        agent.isStopped = false;
        Anim(StaticValue.ANIM_TRIGGER_WALK);
    }

    protected void StopMove()
    {
        isMoving = false;
        agent.SetDestination(targetPos);
        agent.isStopped = true;
        agent.enabled = false;
        Anim(StaticValue.ANIM_TRIGGER_IDLE);
    }
}
