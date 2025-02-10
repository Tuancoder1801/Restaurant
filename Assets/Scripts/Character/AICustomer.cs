using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public LocationTable locationTable;

    public bool isInQueue = true;

    private NavMeshAgent agent;

    private bool isMoving = false;
    private bool hasItem = false;

    public bool isBack = false;
    private bool isEating = false;

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    public override void Update()
    {
        base.Update();
        CheckAnim();
        UpdateWalk();
        UpdateSit();
        UpdateEat();
    }

    public void ChangePos(Transform pos)
    {
        targetPos = pos;
    }

    private void CheckAnim()
    {
        isMoving = targetPos != null;

        hasItem = HasItemInTable();
    }

    #region Idle

    public override void UpdateIdle()
    {
        if (state != CharacterState.Idle) return;

        agent.baseOffset = 0f;

        if (isMoving && !isBack)
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

        if (isInQueue)
        {
            GetToTargetPos(targetPos, CharacterState.Idle);
        }

        if (!isInQueue)
        {
            if (isBack)
            {
                GetToTargetPos(backPos, CharacterState.Idle);
            }
            else
            {
                GetToTargetPos(targetPos, CharacterState.Sit);
            }
        }
    }

    private void GetToTargetPos(Transform target, CharacterState characterState)
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            transform.position = target.position;
            transform.rotation = target.rotation;

            if (target == backPos)
            {
                Destroy(gameObject);
            }
            else
            {
                ChangeState(characterState);
            }
        }
    }

    #endregion

    #region Sit

    public override void UpdateSit()
    {
        if (state != CharacterState.Sit) return;

        agent.baseOffset = -0.3f;

        if (!isInQueue && !isBack)
        {
            if (itemOrders.Count == 0)
            {
                OrderItem();
            }

            if (hasItem)
            {
                ChangeState(CharacterState.Eat);
            }

            if (locationTable != null && locationTable.AllOrdersCompleted())
            {
                //isBack = true;
                //transform.position = locationTable.departurePos.position;
                //transform.rotation = locationTable.departurePos.rotation;
                //ChangeState(CharacterState.Walk);

                LeaveAfterDelay();
            }
        }
        
    }

    private async void LeaveAfterDelay()
    {
        //Debug.Log("Waiting for 3 seconds...");

        isBack = true;
        await Task.Delay(3000);
        //Debug.Log("Delay completed!");

        transform.position = locationTable.departurePos.position;
        transform.rotation = locationTable.departurePos.rotation;
        ChangeState(CharacterState.Walk);
    }

    private void OrderItem()
    {
        itemOrders.Clear();

        List<ItemId> availableItems = new List<ItemId>(FindObjectOfType<AIManager>().GetAvailableItems());

        if (availableItems.Count == 0) return;

        int orderCount = Mathf.Min(Random.Range(1, 2), availableItems.Count);

        for (int i = 0; i < orderCount; i++)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            ItemId selectedItemId = availableItems[randomIndex];

            ItemOrder newOrder = new ItemOrder
            {
                itemId = selectedItemId,
                quantity = Random.Range(1, 3),
                currentItemNumber = 0,
            };

            itemOrders.Add(newOrder);
            availableItems.RemoveAt(randomIndex);
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
            EatItems();          
        }
    }

    private async void EatItems()
    {
        for (int i = 0; i < locationTable.transforms.Count; i++)
        {
            if (locationTable.transforms[i].childCount > 0)
            {
                Transform itemTransform = locationTable.transforms[i].GetChild(0);
                TakeItems(itemTransform);

                await Task.Delay(2000);
            }
        }

        isEating = false;
        ChangeState(CharacterState.Sit);
    }

    #endregion

    private void TakeItems(Transform transform)
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

    private bool HasItemInTable()
    {
        if (locationTable != null)
        {
            for (int i = 0; i < locationTable.itemOrders.Count; i++)
            {
                if (locationTable.itemOrders[i].currentItemNumber > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }
}


