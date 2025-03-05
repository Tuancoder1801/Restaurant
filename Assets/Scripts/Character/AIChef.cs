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
            Debug.Log("isMoving");
            if(Vector3.Distance(transform.position, targetPos) < 0.1f)
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
