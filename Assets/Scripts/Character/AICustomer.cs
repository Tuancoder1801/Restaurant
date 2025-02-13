using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

public enum AICustomerState
{
    START,
    LINEUP,
    MOVETOTABLE,
    EATING,
    FINISH
}

public class AICustomer : AICharacter
{
    public Transform itemIndex;

    public List<ItemOrder> itemOrders = new List<ItemOrder>();
    public LocationTable locationTable;

    public AICustomerState state;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Update()
    {
        ChangeState();
    }

    private void ChangeState()
    {
        if (isMoving) 
        {

        }
        else
        {
            switch (state)
            {
                case AICustomerState.START:
                    break;
                case AICustomerState.LINEUP:
                    break;
                case AICustomerState.MOVETOTABLE:
                    break;
                case AICustomerState.EATING:
                    break;
                case AICustomerState.FINISH:
                    break;
            }

        }
    }

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


