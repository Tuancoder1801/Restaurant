using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum HumanType
{
    NONE = 0,
    //PLAYER = 1, // Player
    CHEF, // đầu bếp, đứng vận hành máy
    WAITER, // nhân viên phục vụ bàn
    PORTER, // phụ bếp
    CUSTOMER, // khách hàng - mua hàng
    COLLECTOR, // Máy hút tiền
}

public class AICharacter : Character
{   
    public int posIndex;

    protected NavMeshAgent agent;
    
    protected Vector3 targetPos;
    protected bool isMoving;

    protected virtual void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    protected void MoveToTarget(Vector3 pos)
    {
        targetPos = pos;

        isMoving = true;
        agent.enabled = true;
        agent.destination = targetPos;
        agent.isStopped = false;
        PlayAnimMove();
    }

    protected void StopMove()
    {
        isMoving = false;
        agent.destination = transform.position;
        agent.isStopped = true;
        agent.enabled = false;
        PlayAnimIdle();
    }
}
