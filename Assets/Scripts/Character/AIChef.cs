using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChef : AICharacter
{
    public LocationMachine machine;

    protected override void OnEnable()
    {   
        base.OnEnable();
        
        isMoving = false;

        var transform = GameManager.Instance.GetTransformCustomer(posIndex);
        this.transform.position = transform.position;

        MoveToTarget(machine.posChef.position);
    }

    protected void Update()
    {
        if (isMoving)
        {
            if (agent.pathPending) return;

            if (agent.remainingDistance <= 0.1f && agent.hasPath)
            {
                StopMove();
                LeanTween.rotate(gameObject, machine.posChef.eulerAngles, 0.3f);
                machine.AddChef(gameObject);
            }
        }
    }
    
    public void PlayAnimCook()
    {
        PlayAnim(StaticValue.ANIM_TRIGGER_COOK);
    }

    public void StopAnimCook()
    {
        PlayAnim(StaticValue.ANIM_TRIGGER_IDLE);
    }
}
