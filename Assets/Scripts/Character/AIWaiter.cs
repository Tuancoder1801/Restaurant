using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public enum AIWaiterState
{
    NONE = 0,
    IDLE = 1,
    MACHINE = 2,
    TABLE = 3,
    TRASH = 4,
}

public class AIWaiter : AICharacter
{
    public Transform transIdle;
    public float timeRequest;

    public List<LocationTable> locationTables;
    public List<LocationMachine> locationMachines;

    private Transform trans;

    private LocationTable currentTable;
    private LocationMachine currentMachine;
    private LocationRawBin currentTrash;

    private float minDistance;
    private float lastDistance = 999;

    private bool isItemRunning;
    private AIWaiterState state = AIWaiterState.NONE;

    public ItemOrder currentOrder = null;

    protected override void OnEnable()
    {
        base.OnEnable();

        isMoving = false;
        isItemRunning = false;

        trans = GameManager.Instance.GetTransformCustomer();
        transform.position = trans.position;

        InitLocationTarget();
    }

    protected void Update()
    {

    }

    private void InitLocationTarget()
    {
        if (locationMachines != null && locationMachines.Count > 0) return;

        var locations = GameManager.Instance.locations;

        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].locationId == LocationId.Machine)
            {
                locationMachines.Add((LocationMachine)locations[i]);
            }
        }
    }

    private void UpdateIdle()
    {
        NextOrder();
    }

    private void UpdateMachine()
    {
        if(isItemRunning || (currentMachine.goPlayer != null)) return;

        var item = currentMachine.PopItem();
        if(item != null)
        {
            isItemRunning = true;
            currentOrder.quantity--;
        }
    }

    private void MoveToPosIdle()
    {
        if (state == AIWaiterState.IDLE) return;
        state = AIWaiterState.IDLE;

        timeCount = 2f;
        currentTable = null;
        currentMachine = null;
        currentTrash = null;
        minDistance = 0.1f;

        MoveToTarget(transIdle.position);
    }

    private void MoveToPosMachine()
    {
        state = AIWaiterState.MACHINE;
        timeCount = timeRequest;
        minDistance = 0f;

        MoveToTarget(currentTable.GetPosProduct());
    }

    private void MoveToTrash()
    {
        state = AIWaiterState.TRASH;
        timeCount = timeRequest;

        //currentTrash = GameManager.Instance;
    }

    private void NextOrder()
    {
        currentTable = null;
        currentMachine = null;
        currentOrder = null;

        var targetTables = locationTables.Where(x => x.gameObject.activeSelf && x.itemOrders != null).ToList();

        if (targetTables != null && targetTables.Count > 0)
        {
            foreach (var table in targetTables)
            {
                if (currentOrder == null)
                {
                    foreach (var order in table.itemOrders)
                    {
                        if (order.currentItemNumber < order.quantity)
                        {
                            var machine = FindMachineNearest(order.itemId);

                            if(machine != null)
                            {
                                currentMachine = machine;
                                currentTable = table;
                                currentOrder = new ItemOrder
                                {
                                    itemId = order.itemId,
                                    quantity = order.quantity - order.currentItemNumber,
                                };
                                break;
                            }
                        }
                    }
                }
                else
                {
                    var order = table.itemOrders.FirstOrDefault(x => x.itemId == currentOrder.itemId);
                    if(order != null)
                    {
                        currentOrder.quantity += (order.quantity - order.currentItemNumber);
                    }
                }
            }
        }

        if(currentTable == null)
        {
            MoveToPosIdle();
        }
        else
        {
            MoveToPosMachine();
        }
    }

    private LocationMachine FindMachineNearest(ItemId itemId)
    {
        List<LocationMachine> machineHasProducts = new List<LocationMachine>();
        List<LocationMachine> machineWillHaveProduct = new List<LocationMachine>();
        locationMachines.ForEach(x => {
            if (x.product.itemId == itemId && x.gameObject.activeSelf)
            {
                if (x.product.IsHasItem()) machineHasProducts.Add(x);
                else if (machineHasProducts.Count == 0 && x.MaxProductCanMake() > 0) machineWillHaveProduct.Add(x);
            }
        });

        if (machineHasProducts.Count == 0) machineHasProducts = machineWillHaveProduct;

        LocationMachine machine = null;
        float distance = 999999;
        float d = 0;
        machineHasProducts.ForEach(x => {
            d = Vector3.Distance(transform.position, x.transform.position);
            if (d < distance)
            {
                distance = d;
                machine = x;
            }
        });

        return machine;
    }

    public override void SetIdleTransform(Transform tran)
    {
        if (transIdle == null) transIdle = tran;
    }
}