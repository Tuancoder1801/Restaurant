using System.Collections;
using System.Collections.Generic;
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
    private LocationMachine currentKitchen;
    private RawBin currentTrash;

    private float minDistance;
    private float lastDistance = 999;

    private bool isItemRunning;
    private AIWaiterState state =  AIWaiterState.NONE;

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
        if(locationMachines != null && locationMachines.Count > 0) return;

        var locations = GameManager.Instance.locations;

        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].locationId == LocationId.Machine)
            {
                locationMachines.Add((LocationMachine)locations[i]);
            }
        }
    }

    private void MoveToPosIdle()
    {
        if(state == AIWaiterState.IDLE)  return;
        state = AIWaiterState.IDLE;

        timeCount = 2f;
        currentTable = null;
        currentKitchen = null;
        currentTrash = null;
        minDistance = 0.1f;

        MoveToTarget(transIdle.position);
    }

    private void MoveToPosMachine()
    {
        state = AIWaiterState.MACHINE;
        timeCount = timeRequest;
        minDistance = currentKitchen.GetBoxRange();

        MoveToTarget(currentTable.GetPosProduct());
    }

    private void MoveToTrash()
    {

    }
}