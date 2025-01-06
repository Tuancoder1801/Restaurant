using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    Idle,
    Run,
    IdleHold,
    RunHold
}

public class Character : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;
    public CharacterState state = CharacterState.Idle;
    //public Rigidbody rb;

    private Animator animator;

    public virtual void Awake()
    {
        //animator = GetComponent<Animator>();
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void Update()
    {
        UpdateIdle();
        UpdateRun();
        UpdateIdleHold();
        UpdateRunHold();
    }

    public virtual void ChangeState(CharacterState newState)
    {
        if (state != newState)
        {
            switch (newState)
            {
                case CharacterState.Idle:
                    BeginIdle();
                    break;
                case CharacterState.Run:
                    BeginRun();
                    break;
                case CharacterState.IdleHold:
                    BeginIdleHold();
                    break;
                case CharacterState.RunHold:
                    BeginRunHold();
                    break;
            }

        }
    }

    private void ResetAllTriggers()
    {
        animator.ResetTrigger(StaticValue.ANIM_TRIGGER_IDLE);
        animator.ResetTrigger(StaticValue.ANIM_TRIGGER_RUN);
        animator.ResetTrigger(StaticValue.ANIM_TRIGGER_IDLE_HOLD);
        animator.ResetTrigger(StaticValue.ANIM_TRIGGER_RUN_HOLD);
    }

    #region Idle

    public virtual void BeginIdle()
    {
        ResetAllTriggers();
        state = CharacterState.Idle;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_IDLE);
    }

    public virtual void UpdateIdle()
    {

    }

    public virtual void BeginIdleHold()
    {
        ResetAllTriggers();
        state = CharacterState.IdleHold;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_IDLE_HOLD);
    }

    public virtual void UpdateIdleHold()
    {
    }

    #endregion

    #region Run

    public virtual void BeginRun()
    {
        ResetAllTriggers();
        state = CharacterState.Run;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_RUN);
    }

    public virtual void UpdateRun()
    {

    }

    public virtual void BeginRunHold()
    {
        ResetAllTriggers();
        state = CharacterState.RunHold;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_RUN_HOLD);
    }

    public virtual void UpdateRunHold()
    {
    }

    #endregion
}
