using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChef : AICharacter
{
    public KitchenTable kitchen;

    protected override void OnEnable()
    {   
        base.OnEnable();
        
        isMoving = false;

        var transform = GameManager.Instance.GetTransformCustomer(posIndex);
        this.transform.position = transform.position;

        MoveToTarget(kitchen.chefIndex.position);
    }

    protected void Update()
    {
        if (isMoving)
        {
            if(Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                StopMove();
                LeanTween.rotate(gameObject, kitchen.chefIndex.eulerAngles, 0.3f);
            }
        }
    }
    
    public void PlayAnimCook()
    {
        Anim(StaticValue.ANIM_TRIGGER_COOK);
    }

    public void StopAnimCook()
    {
        Anim(StaticValue.ANIM_TRIGGER_IDLE);
    }
}
