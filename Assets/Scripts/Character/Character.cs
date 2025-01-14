using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    Idle,
    Run,
    IdleHold,
    RunHold,
    Walk,
    WalkHold,
    Sit,
    Eat,
    Cook,
}

public class Character : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;
    public CharacterState state = CharacterState.Idle;

    public int maxStackNumber = 3;
    public List<Transform> itemTransforms = new List<Transform>();
    public int currentItemNumber = 0;
    public bool isHolding = false;

    private Animator animator;

    public virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void Update()
    {
        UpdateIdle();
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
                case CharacterState.Walk:
                    BeginWalk();
                    break;
                case CharacterState.WalkHold:
                    BeginWalkHold();
                    break;
                case CharacterState.Sit:
                    BeginSit();
                    break;
                case CharacterState.Eat:
                    BeginEat();
                    break;
                case CharacterState.Cook:
                    BeginCook();
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
        animator.ResetTrigger(StaticValue.ANIM_TRIGGER_WALK);
        animator.ResetTrigger(StaticValue.ANIM_TRIGGER_WALK_HOLD);
        animator.ResetTrigger(StaticValue.ANIM_TRIGGER_SIT);
    }

    #region Idle

    public void BeginIdle()
    {
        ResetAllTriggers();
        state = CharacterState.Idle;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_IDLE);
    }

    public virtual void UpdateIdle()
    {

    }

    public void BeginIdleHold()
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

    public void BeginRun()
    {
        ResetAllTriggers();
        state = CharacterState.Run;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_RUN);
    }

    public virtual void UpdateRun()
    {

    }

    public void BeginRunHold()
    {
        ResetAllTriggers();
        state = CharacterState.RunHold;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_RUN_HOLD);
    }

    public virtual void UpdateRunHold()
    {
    }

    #endregion

    #region Walk

    public void BeginWalk()
    {
        ResetAllTriggers();
        state = CharacterState.Walk;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_WALK);
    }

    public virtual void UpdateWalk()
    {

    }

    public void BeginWalkHold()
    {
        ResetAllTriggers();
        state = CharacterState.WalkHold;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_WALK_HOLD);
    }

    public virtual void UpdateWalkHold()
    {

    }

    #endregion

    #region Sit

    public void BeginSit()
    {
        ResetAllTriggers();
        state = CharacterState.Sit;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_SIT);
    }

    public virtual void UpdateSit()
    {

    }

    #endregion

    #region Eat

    public void BeginEat()
    {
        ResetAllTriggers();
        state = CharacterState.Eat;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_EAT);
    }

    public virtual void UpdateEat()
    {

    }

    #endregion

    #region Cook

    public void BeginCook()
    {
        ResetAllTriggers();
        state = CharacterState.Cook;
        animator.SetTrigger(StaticValue.ANIM_TRIGGER_COOK);
    }

    public virtual void UpdateCook()
    {

    }

    #endregion

    public void ReceiveItems(BaseItem item)
    {
        if (currentItemNumber >= maxStackNumber) return;
        
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

    public void ReleaseItems(List<ItemPosition> itemPositions)
    {
        if (currentItemNumber <= 0 || currentItemNumber > itemTransforms.Count) return;

        for(int i = 0; i < itemPositions.Count; i++)
        {
            if (!itemPositions[i].CheckMaxStack()) return;
        }

        Sequence sequence = DOTween.Sequence();
        Transform item = itemTransforms[currentItemNumber - 1].GetChild(0);
        ItemId itemId = item.GetComponent<BaseItem>().itemId;

        foreach (var itemPosition in itemPositions)
        {
            if (itemPosition.itemId == itemId)
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
