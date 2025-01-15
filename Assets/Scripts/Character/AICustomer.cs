using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AICustomer : Character
{
    public Transform sittingPos;
    public Transform startPos;
    public Transform targetPos;
    public Transform departurePos;

    private NavMeshAgent agent;
    private bool isStartPos = false;
    private bool isBackStart = false;

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    public override void Update()
    {
        base.Update();
        UpdateWalk();
        UpdateSit();
    }

    public void ChangePos(Transform pos)
    {
        startPos = pos;
    }

    #region Idle

    public override void UpdateIdle()
    {
        if (state != CharacterState.Idle) return;
        
        if(startPos != null && !isStartPos)
        {
            ChangeState(CharacterState.Walk);
        }

        if (isStartPos && targetPos == null)
        {
            GameObject targetObject = GameObject.FindWithTag("target");
            if (targetObject != null)
            {
                agent.isStopped = false;
                targetPos = targetObject.transform;
                ChangeState(CharacterState.Walk);
            }
        }
    }

    #endregion

    #region Walk

    public override void UpdateWalk()
    {
        if (state != CharacterState.Walk) return;

        if(!isStartPos && startPos != null)
        {   
            GetToTargetPos(startPos, startPos, CharacterState.Idle);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isStartPos = true;
                ChangeState(CharacterState.Idle);
            }
        }

        if (isStartPos && targetPos != null)
        {
            GetToTargetPos(targetPos, sittingPos, CharacterState.Sit);
        }

        if (isBackStart)
        {
            isStartPos = false;
            agent.isStopped = false;
            GetToTargetPos(startPos, startPos, CharacterState.Idle);
        }
    }

    private void GetToTargetPos(Transform target, Transform actionPos, CharacterState state)
    {
        agent.SetDestination(target.position);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {   
            agent.isStopped = true;
            transform.position = actionPos.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, actionPos.rotation, agent.angularSpeed * Time.deltaTime);
            Debug.Log(Quaternion.Angle(transform.rotation, actionPos.rotation));
            if (Quaternion.Angle(transform.rotation, actionPos.rotation) < 1f)
            {
                ChangeState(state);
            }
        }
    }

    #endregion

    #region Sit

    public override void UpdateSit()
    {
        if (state != CharacterState.Sit) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            isBackStart = true;
            transform.position = departurePos.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, departurePos.rotation, agent.angularSpeed * Time.deltaTime);
            Debug.Log(Quaternion.Angle(transform.rotation, departurePos.rotation));
            if (Quaternion.Angle(transform.rotation, departurePos.rotation) < 1f)
            {
                ChangeState(CharacterState.Walk);
            }
        }
    }

    private void OrderItem()
    {

    }

    #endregion

    #region Eat

    public override void UpdateEat()
    {

    }

    #endregion
}
