using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

public class AICustomer : Character
{
    public Transform targetPos;
    public Transform backPos;
    public Transform itemIndex;

    public List<ItemOrder> itemOrders = new List<ItemOrder>();
    public LocationTable locationTable = null;

    public bool isInQueue = true;

    private NavMeshAgent agent;
    private bool isBack = false;
    private bool isEating = false;

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
        UpdateEat();
    }

    public void ChangePos(Transform pos)
    {
        targetPos = pos;
    }

    #region Idle

    public override void UpdateIdle()
    {
        if (state != CharacterState.Idle) return;

        agent.baseOffset = 0f;

        if (targetPos != null)
        {
            ChangeState(CharacterState.Walk);
        }
    }

    #endregion

    #region Walk

    public override void UpdateWalk()
    {
        if (state != CharacterState.Walk) return;

        agent.baseOffset = 0f;

        if (isBack)
        {
            agent.isStopped = false;
            GetToTargetPos(backPos);
            return;
        }

        if (isInQueue)
        {
            agent.isStopped = false;
            GetToTargetPos(targetPos);
            return;
        }

        if (!isInQueue)
        {
            agent.isStopped = false;
            GetToTargetPos(targetPos);
            return;
        }
    }

    private void GetToTargetPos(Transform target)
    {
        agent.SetDestination(target.position);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            targetPos = null;
            transform.position = target.position;
            transform.rotation = target.rotation;

            if (!isInQueue)
            {
                ChangeState(CharacterState.Sit);
            }
            else
            {
                ChangeState(CharacterState.Idle);
            }
        }
    }

    #endregion

    #region Sit

    public override void UpdateSit()
    {
        if (state != CharacterState.Sit) return;

        agent.baseOffset = -0.3f;

        if (itemOrders.Count == 0)
        {
            OrderItem();
        }

        if (locationTable != null && locationTable.AllOrdersCompleted() /*&& !isEating*/)
        {
            if (!isBack)
            {
                StartCoroutine(LeaveAfterDelay());
            }
        }

        if (locationTable != null && HasValidItems(locationTable.transforms))
        {
            Debug.Log("ngoi");
            ChangeState(CharacterState.Eat);
        }
    }

    private IEnumerator LeaveAfterDelay()
    {
        yield return new WaitForSeconds(0.4f);

        targetPos = null;
        transform.position = locationTable.departurePos.position;
        transform.rotation = locationTable.departurePos.rotation;

        isBack = true;

        ChangeState(CharacterState.Walk);
    }

    private bool HasValidItems(List<Transform> items)
    {
        foreach (var item in items)
        {
            if (item.childCount > 0) // Kiểm tra item không null
            {
                return true; // Có ít nhất một item hợp lệ
            }
        }
        return false; // Không có item nào hợp lệ
    }

    private void OrderItem()
    {
        itemOrders.Clear();

        for (int i = 0; i < Random.Range(1, 2); i++)
        {
            int randomIndex = Random.Range(0, GameDataConstant.products.Count);

            ItemOrder newOrder = new ItemOrder
            {
                itemId = GameDataConstant.products[randomIndex].itemId,
                quantity = Random.Range(2, 2),
                currentItemNumber = 0,
            };

            itemOrders.Add(newOrder);
        }
    }

    public List<ItemOrder> GetOrder()
    {
        return itemOrders;
    }

    #endregion

    #region Eat

    public void UpdateEat()
    {
        if (state != CharacterState.Eat) return;

        if (locationTable != null && !isEating)
        {
            isEating = true;
            StartCoroutine(EatItems());
        }
    }

    private IEnumerator EatItems()
    {
        for (int i = 0; i < locationTable.transforms.Count; i++)
        {
            if (locationTable.transforms[i].childCount > 0)
            {
                Transform itemTransform = locationTable.transforms[i].GetChild(0);
                TakeItems(itemTransform);
                yield return new WaitForSeconds(2f);
            }
        }

        isEating = false;
        ChangeState(CharacterState.Sit);
    }

    #endregion

    public void TakeItems(Transform transform)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        BaseItem item = transform.GetComponent<BaseItem>();

        if (item == null) return;

        sequence.Append(
        item.transform.DOMove(itemIndex.position, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(item.gameObject);
        })
    );
    }
}


