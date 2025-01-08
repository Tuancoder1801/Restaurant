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

    public int maxStackNumber = 3;
    public List<Transform> itemTransforms = new List<Transform>();
    public int currentItemNumber = 0;

    private bool isMoving = false;
    private float valueVertical;
    private float valueHorizontal;
    private bool isHolding = false;

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

    public void ReceiveItems(BaseItem item)
    {
        if (currentItemNumber >= maxStackNumber)
        {
            Debug.LogWarning("Cannot receive more items, stack is full.");
            return;
        }

        isHolding = true;
        Sequence sequence = DOTween.Sequence();

        float accumulatedHeight = 0f;

        for (int i = 0; i < currentItemNumber; i++)
        {
            BaseItem placedItem = itemTransforms[i].GetComponentInChildren<BaseItem>();
            if (placedItem != null)
            {
                accumulatedHeight += placedItem.height;
            }
        }

        Vector3 currentTransformPosition = itemTransforms[currentItemNumber].localPosition;
        currentTransformPosition.y = itemTransforms[0].localPosition.y + accumulatedHeight;
        itemTransforms[currentItemNumber].localPosition = new Vector3(currentTransformPosition.x, currentTransformPosition.y, currentTransformPosition.z);

        sequence.Append(
            item.transform.DOJump(itemTransforms[currentItemNumber].position, 1f, 1, 0.2f).OnComplete(() =>
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

        Sequence sequence = DOTween.Sequence();
        Transform item = itemTransforms[currentItemNumber - 1].GetChild(0);
        ItemId itemId = item.GetComponent<BaseItem>().itemId;

        foreach (var itemPosition in itemPositions)
        {
            if (itemPosition.itemId == itemId && itemPosition.currentStackNumber < maxStack)
            {
                sequence.Append(
                    item.DOJump(itemPosition.itemPositions[itemPosition.currentStackNumber].position, 1f, 1, 0.2f).OnComplete(() =>
                    {
                        item.SetParent(itemPosition.itemPositions[itemPosition.currentStackNumber]);
                        item.localPosition = Vector3.zero;
                        item.localRotation = Quaternion.identity;
                        item.localScale = Vector3.one;

                        currentItemNumber--;
                        itemPosition.currentStackNumber++;
                        ResetItemPositions(currentItemNumber);
                        if (currentItemNumber == 0) isHolding = false;
                    })
                    );
            }
        }
    }

    private void ResetItemPositions(int index)
    {
        if (index < 0 || index >= itemTransforms.Count)
        {
            return;
        }

        Vector3 resetPosition = itemTransforms[index].localPosition;
        resetPosition.y = itemTransforms[0].localPosition.y;
        itemTransforms[index].localPosition = resetPosition;
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
        item.DOJump(targetTransform.position, 1f, 1, 0.2f).OnComplete(() =>
        {
            Destroy(item.gameObject);
            currentItemNumber--;

            ResetItemPositions(currentItemNumber);

            if (currentItemNumber == 0) isHolding = false;
        })
        );

    }
}
