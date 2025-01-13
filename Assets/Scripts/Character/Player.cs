using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEditor.Progress;
using UnityEngine.TextCore.Text;
using DG.Tweening.Core.Easing;
using static UnityEngine.GraphicsBuffer;

public class Player : Character
{
    public Joystick joystick;
    public CharacterController character;

    private bool isMoving = false;
    private float valueVertical;
    private float valueHorizontal;

    public override void Awake()
    {
        base.Awake();
        joystick = FindObjectOfType<Joystick>();
    }

    public override void Update()
    {
        CheckInput();
        base.Update();
        UpdateRun();
        UpdateIdleHold();
        UpdateRunHold();
    }

    private void CheckInput()
    {
        valueVertical = joystick.Vertical;
        valueHorizontal = joystick.Horizontal;

        isMoving = valueVertical != 0 || valueHorizontal != 0;
    }

    #region Idle

    public override void UpdateIdle()
    {
        if (state != CharacterState.Idle) return;

        if (isMoving)
        {
            ChangeState(CharacterState.Run);
        }

        if (isHolding)
        {
            ChangeState(CharacterState.IdleHold);
        }
    }

    public override void UpdateIdleHold()
    {
        if (state != CharacterState.IdleHold) return;

        if (!isHolding)
        {
            ChangeState(CharacterState.Idle);
        }

        if (isMoving && isHolding)
        {
            ChangeState(CharacterState.RunHold);
        }
    }

    #endregion  

    #region Run

    public override void UpdateRun()
    {
        if (state != CharacterState.Run) return;

        if (!isMoving)
        {
            if (isHolding)
            {
                ChangeState(CharacterState.IdleHold);
                return;
            }

            ChangeState(CharacterState.Idle);
        }

        if (isHolding)
        {
            ChangeState(CharacterState.RunHold);
        }

        HandleMovement();

    }

    public override void UpdateRunHold()
    {
        if (state != CharacterState.RunHold) return;

        if (isHolding && !isMoving)
        {
            ChangeState(CharacterState.IdleHold);
        }

        if (!isHolding && isMoving)
        {
            ChangeState(CharacterState.Run);
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 direction = Vector3.forward * valueVertical + Vector3.right * valueHorizontal;
        direction = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * direction;

        character.Move(direction * Time.deltaTime * moveSpeed);

        if (direction != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }

        var tempVector3 = transform.position;
        tempVector3.y = 0;
        transform.position = tempVector3;
    }

    #endregion

}
