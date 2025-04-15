using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Terrain;

public enum AIPorterState
{
    NONE = 0,
    IDLE,
    RAWBIN,
    MACHINE,
    TRASH
}

public class AIPorter : AICharacter
{
    public List<ItemId> taskItems = new List<ItemId>();
    public Transform transIdle;

    public float timeRequest;

    public List<LocationBase> rawBins = new List<LocationBase>();
    public List<LocationMachine> machines = new List<LocationMachine>();

    private Transform tran;

    private LocationBase currentlocation;
    private LocationMachine currentMachine;
    private LocationTrash currentTrash;

    private ItemId itemId;
    private int needItemQuantity;

    private float minDistance;
    private float lastDistance = 999;

    private bool isItemRuning;
    private AIPorterState state = AIPorterState.NONE;

    protected override void OnEnable()
    {
        base.OnEnable();

        isMoving = false;
        isItemRuning = false;

        tran = GameManager.Instance.GetTransformCustomer();
        transform.position = tran.position;

        InitLocationTarget();
        MoveToPosIdle();
    }

    protected void Update()
    {
        if (isMoving)
        {
            var d = Vector3.Distance(transform.position, targetPos);
            if (d < minDistance || (d < 4f && lastDistance == d))
            {
                StopMove();
                LeanTween.rotate(gameObject, transIdle.eulerAngles, 0.3f);
            }
            else
            {
                lastDistance = d;
            }
        }
        else
        {
            timeCount -= Time.deltaTime;
            if (timeCount <= 0)
            {
                timeCount = timeRequest;
                switch (state)
                {
                    case AIPorterState.IDLE:
                        timeCount = 2f;
                        UpdateIdle();
                        break;
                    case AIPorterState.RAWBIN:
                        UpdateMaterial();
                        break;
                    case AIPorterState.MACHINE:
                        UpdateMachine();
                        break;
                    case AIPorterState.TRASH:
                        UpdateTrash();
                        break;
                }
            }
        }
    }

    private void InitLocationTarget()
    {
        if (machines != null && rawBins != null && machines.Count > 0 && rawBins.Count > 0)
        {
            return;
        }

        var locations = GameManager.Instance.AllLocation;

        List<ItemId> materials = new List<ItemId>();
        List<LocationBase> materialTemps = new List<LocationBase>();

        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].locationId == LocationId.Machine)
            {
                var machine = (LocationMachine)locations[i];
                materialTemps.Add(machine);

                if (taskItems.Contains(machine.GetProductId()) && machine.materials != null && machine.materials.Count > 0)
                {
                    machines.Add(machine);
                    machine.materials.ForEach(x => materials.Add(x.itemId));
                }
            }
            else if (locations[i].locationId == LocationId.RawBin)
            {
                materialTemps.Add(locations[i]);
            }
        }

        for (int i = 0; i < materialTemps.Count; i++)
        {
            var material = materialTemps[i];

            if (materials.Contains(material.GetProductId()))
            {
                rawBins.Add(material);
            }
        }
    }

    private void UpdateIdle()
    {
        NextOrder();
    }

    private void UpdateMaterial()
    {
        if (isItemRuning || (currentlocation.goPlayer != null && !GameManager.Instance.player.IsFullStack())) return;

        var item = currentlocation.PopItem();
        if (item != null)
        {
            isItemRuning = true;
            needItemQuantity--;

            PushItem(item, () =>
            {
                if (IsFullStack() || needItemQuantity <= 0)
                {
                    MoveToMachine();
                }
                isItemRuning = false;
            });

            PlayAnimIdle();
        }
        else
        {
            var locationNew = FindLocationNearest(itemId);

            if (locationNew == null)
            {
                if (!IsEmpty()) MoveToMachine();
                else NextOrder();
            }
            else if (locationNew != currentlocation)
            {
                currentlocation = locationNew;
                MoveToLocation();
            }
        }
    }

    private void UpdateMachine()
    {
        if (IsEmpty())
        {
            NextOrder();
        }
        else
        {
            var material = currentMachine.GetItemMaterial(itemId);

            if (material == null || material.IsFullStack())
            {
                MoveToTrash();
            }
            else
            {
                var item = PopItem();
                currentMachine.PushItem(item);
                PlayAnimIdle();
            }
        }
    }

    private void UpdateTrash()
    {
        if (IsEmpty())
        {
            NextOrder();
        }
        else
        {
            var item = PopItem();
            if (item != null)
            {
                currentTrash.PushItem(item);
            }
            else
            {
                NextOrder();
            }
        }
    }

    private void MoveToPosIdle()
    {
        if (state == AIPorterState.IDLE) return;

        state = AIPorterState.IDLE;
        timeCount = 2f;
        currentlocation = null;
        currentMachine = null;
        currentTrash = null;
        minDistance = 0.1f;

        MoveToTarget(transIdle.position);
    }

    private void MoveToLocation()
    {
        state = AIPorterState.RAWBIN;

        timeCount = timeRequest;
        minDistance = currentlocation.GetBoxRange();
        MoveToTarget(currentlocation.GetPosRawBin());
    }

    private void MoveToMachine()
    {
        state = AIPorterState.MACHINE;
        timeCount = timeRequest;
        minDistance = currentMachine.GetBoxRange();
        
        MoveToTarget(currentMachine.GetPosRawBin());
    }

    private void MoveToTrash()
    {
        state = AIPorterState.TRASH;
        timeCount = timeRequest;

        currentTrash = GameManager.Instance.GetLocationNearesByItem(LocationId.Trash, transform.position) as LocationTrash;
        minDistance = currentTrash.GetBoxRange();
        MoveToTarget(currentTrash.GetPosProduct());
    }

    private void NextOrder()
    {
        currentMachine = null;
        currentlocation = null;

        LocationMachine machine = null;
        LocationBase material = null;
        int quantity = 0;

        for (int i = 0; i < machines.Count; i++)
        {
            var m = machines[i];
            if (m.gameObject.activeSelf)
            {
                if (m.product.IsEmpty())
                {
                    var tu = m.GetLocationRequire();
                    if (tu.Item1 != ItemId.None && tu.Item2 > 0)
                    {
                        material = FindLocationNearest(tu.Item1);
                        if (material != null)
                        {
                            machine = m;
                            quantity = tu.Item2;
                            break;
                        }
                    }
                }
            }
        }

        if (machine == null)
        {
            for (int i = 0; i < machines.Count; i++)
            {
                var m = machines[i];
                if (m.gameObject.activeSelf)
                {
                    var tu = m.GetLocationRequire();
                    if (tu.Item1 != ItemId.None && tu.Item2 > 0)
                    {
                        material = FindLocationNearest(tu.Item1);
                        if (material != null)
                        {
                            machine = m;
                            quantity = tu.Item2;
                            break;
                        }
                    }
                }
            }
        }

        if (machine != null && material != null)
        {
            currentMachine = machine;
            currentlocation = material;

            needItemQuantity = quantity;
            itemId = currentlocation.GetProductId();

            MoveToLocation();
        }
        else
        {
            MoveToPosIdle();
        }
    }

    private LocationBase FindLocationNearest(ItemId itemType)
    {
        List<LocationBase> machineHasProducts = new List<LocationBase>();
        List<LocationBase> machineWillHaveProduct = new List<LocationBase>();
        rawBins.ForEach(x =>
        {
            if (x.GetProductId() == itemType && x.gameObject.activeSelf)
            {
                if (x.HasProductItem()) machineHasProducts.Add(x);
                else if (machineHasProducts.Count == 0 && x.MaxProductCanMake() > 0) machineWillHaveProduct.Add(x);
            }
        });

        if (machineHasProducts.Count == 0) machineHasProducts = machineWillHaveProduct;

        LocationBase machine = null;
        float distance = 999999;
        float d = 0;
        machineHasProducts.ForEach(x =>
        {
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
