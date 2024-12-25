using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEditor.Progress;

public class Player : Character
{
    public Joystick joystick;

    public int maxStackNumber = 3;
    public List<Transform> itemTransforms = new List<Transform>();

    //public bool isHolding = false;
    public int currentItemNumber = 0;

    private bool isMoving = false;
    private Vector3 moveVector;
    private float valueVertical;
    private float valueHorizontal;

    private Queue<System.Action> actionQueue = new Queue<System.Action>();
    private bool isProcessing = false;
    public bool isHolding = false;


    public override void Awake()
    {
        base.Awake();
        joystick = FindObjectOfType<Joystick>();
    }

    public override void Update()
    {
        CheckInput();
        base.Update();
    }

    private void CheckInput()
    {
        valueVertical = joystick.Vertical;
        valueHorizontal = joystick.Horizontal;

        isMoving = valueVertical != 0 || valueHorizontal != 0;
        //isHolding = currentItemNumber > 0;
    }

    #region Idle

    public override void BeginIdle()
    {
        base.BeginIdle();
    }

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

    public override void BeginIdleHold()
    {
        base.BeginIdleHold();
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

    public override void BeginRun()
    {
        base.BeginRun();
    }

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

    public override void BeginRunHold()
    {
        base.BeginRunHold();
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
        moveVector = Vector3.zero;
        moveVector.x = valueHorizontal * moveSpeed * Time.deltaTime;
        moveVector.z = valueVertical * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveVector);

        Vector3 direction = Vector3.RotateTowards(transform.forward, moveVector, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);
    }

    #endregion

    public void ReceiveItems(BaseItem item)
    {
        if (itemTransforms[currentItemNumber].childCount > 0)
        {
            return;
        }

        isHolding = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(
        item.transform.DOJump(itemTransforms[currentItemNumber].position, 2f, 1, 0.5f).OnComplete(() =>
        {
            item.transform.SetParent(itemTransforms[currentItemNumber]);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;

            currentItemNumber++;

        })
        );

    }

    public void ReleaseItems(List<ItemPosition> itemPositions, int maxStack)
    {

        if (currentItemNumber <= 0 || currentItemNumber > itemTransforms.Count)
        {
            Debug.LogWarning("Invalid currentItemNumber or out of range.");
            return;
        }

        Transform item = itemTransforms[currentItemNumber - 1].GetChild(0);

        ItemId itemId = item.GetComponent<BaseItem>().itemId;
        Sequence sequence = DOTween.Sequence();

        foreach (var itemPosition in itemPositions)
        {
            if (itemPosition.itemId == itemId)
            {
                if (itemPosition.currentStackNumber < maxStack)
                {
                    sequence.Append(
                    item.DOJump(itemPosition.itemPositions[itemPosition.currentStackNumber].position, 2f, 1, 0.5f).OnComplete(() =>
                    {
                        item.SetParent(itemPosition.itemPositions[itemPosition.currentStackNumber]);
                        item.localPosition = Vector3.zero;
                        item.localRotation = Quaternion.identity;
                        item.localScale = Vector3.one;

                        currentItemNumber--;
                        itemPosition.currentStackNumber++;

                        if(currentItemNumber == 0) isHolding = false;
                    })
                    );
                }
            }
        }
    }

    public void DropItems(Transform targetTransform)
    {
        if (currentItemNumber < 0)
        {
            return;
        }

        Sequence sequence = DOTween.Sequence();
        Transform item = itemTransforms[currentItemNumber - 1].GetChild(0);

        sequence.Append(
        item.DOJump(targetTransform.position, 2f, 1, 0.5f).OnComplete(() =>
        {
            Destroy(item.gameObject);
            currentItemNumber--;

            if (currentItemNumber == 0) isHolding = false;
        })
        );

    }

}
