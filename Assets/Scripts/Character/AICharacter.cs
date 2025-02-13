using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacter : MonoBehaviour
{
    public Transform posIdle;

    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform targetPos;

    protected bool isMoving;

    protected virtual void Awake()
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

    protected void MoveToTarget(Transform pos)
    {
        targetPos = pos;

        isMoving = true;
        agent.enabled = true;
        agent.SetDestination(targetPos.position);
        agent.isStopped = false;
    }

    protected void StopMove()
    {
        isMoving = false;
        agent.SetDestination(targetPos.position);
        agent.isStopped = true;
        agent.enabled = false;
    }
}
