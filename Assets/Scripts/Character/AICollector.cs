using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AICollector : AICharacter
{
    public Text txtTime;
    public GameObject goTime;
    public GameObject goEmpty;
    public GameObject goCharging;

    public List<GameObject> goIcons;
    public Transform transIdle;

    public List<CollectorEquipment> skinCollectors;

    private List<SubLocationMoney> locationMoneys;
    private List<Transform> wayPoints;
    private SubLocationMoney locationCurrent;

    [HideInInspector]
    public float timeCurrent;
    private float timeTemp;
    private TimeSpan timeSpan;
    private string strTimeFormat = "{0:00}m {1:00}s";

    public bool isWorking;
    public bool isUI = false;

    private Transform curWayPoint;

    public CollectorEquipment collectorEquipment;

    protected override void OnEnable()
    {
        base.OnEnable();

        isMoving = false;
        isWorking = false;

        if (!isUI)
        {
            transform.position = transIdle.position;
            transform.eulerAngles = transIdle.eulerAngles;
            wayPoints = GameManager.Instance.transColectorWayPoints;
            curWayPoint = null;

            InitLocationTarget();
        }
    }

    protected void Update()
    {
        if (isUI) return;

        if (isMoving)
        {
            if (isWorking && curWayPoint != null)
            {
                timeCount--;
                if (timeCount <= 0)
                {
                    timeCount = 0.5f;
                    if (locationMoneys.Any(x => x.currentMoney > 0 && x.gameObject.activeSelf))
                    {
                        NextTask();
                    }
                }
            }

            var d = Vector3.Distance(transform.position, targetPos);
            if (d < 0.2f)
            {
                StopMove();
            }
        }
        else
        {
            if (isWorking)
            {
                timeCount--;
                if (timeCount <= 0)
                {
                    timeCount = 0.5f;
                    if (locationCurrent == null || locationCurrent.currentMoney <= 0)
                    {
                        NextTask();
                    }
                }
            }
            else
            {
                if (timeCurrent <= 0)
                {  
                    LeanTween.rotate(gameObject, transIdle.transform.eulerAngles, 0.3f);
                    SetWorking(false, 10f);
                }
            }
        }

        // count time 
        if (timeCurrent > 0)
        {
            timeTemp += Time.deltaTime;

            if (timeTemp >= 1)
            {
                timeTemp = 0;
                timeCurrent--;

                if (timeCurrent <= 0)
                {
                    txtTime.text = "";
                    if (isWorking)
                    {
                        MoveToPosIdle();
                    }
                    else
                    {
                        SetWorking(true, 30f);
                        NextTask();
                    }
                }
                else
                {
                    ShowTime(timeCurrent);
                }
            }
        }
    }

    private void InitLocationTarget()
    {
        if(locationMoneys != null && locationMoneys.Count > 0)
        {
            return;
        }

        locationMoneys = new List<SubLocationMoney>();
        var locations = GameManager.Instance.locations;

        for(int i = 0; i < locations.Count; i++)
        {
            if (locations[i].locationId == LocationId.Table)
            {
                var table = (LocationTable)locations[i];
                if(table.locationMoney != null) locationMoneys.Add(table.locationMoney);
            }
        }
    }

    public void SetWorking(bool working, float time)
    {
        timeTemp = 0;
        isWorking = working;
        timeCurrent = time;
        curWayPoint = null;
        ShowTime(timeCurrent);
    }

    private void NextTask()
    {
        locationCurrent = null;
        float distance = 99999f;

        for(int i = 0; i < locationMoneys.Count; i++)
        {
            if (locationMoneys[i].gameObject.activeSelf && locationMoneys[i].currentMoney > 0)
            {
                float dis = Vector3.Distance(transform.position, locationMoneys[i].tranMoney.position);

                if(dis < distance)
                {
                    locationCurrent = locationMoneys[i];
                    distance = dis;
                }
            }
        }

        if (locationCurrent != null)
        {
            curWayPoint = null;
            MoveToLocation();
        }
        else
        {
            // move waypoint
            if (curWayPoint != null)
            {
                int index = wayPoints.IndexOf(curWayPoint);
                index++;
                if (index >= wayPoints.Count) index = 0;
                curWayPoint = wayPoints[index];
            }
            else
            {
                distance = 99999f;
                for (int i = 0; i < wayPoints.Count; i++)
                {
                    float dis = Vector3.Distance(transform.position, wayPoints[i].position);
                    if (dis < distance)
                    {
                        curWayPoint = wayPoints[i];
                        distance = dis;
                    }
                }
            }

            if (curWayPoint != null) MoveToTarget(curWayPoint.position);
        }
    }

    private void MoveToPosIdle()
    {
        goTime.SetActive(false);
        goEmpty.SetActive(true);
        goCharging.SetActive(false);

        isWorking = false;
        locationCurrent = null;

        MoveToTarget(transIdle.position);
    }

    private void MoveToLocation()
    {
        timeCount = 0f;
        MoveToTarget(locationCurrent.tranMoney.position);
    }

    private void ShowTime(float timeShow)
    {
        goTime.SetActive(true);
        goEmpty.SetActive(false);
        goCharging.SetActive(!isWorking);
        timeSpan = TimeSpan.FromSeconds(timeShow);
        txtTime.text = string.Format(strTimeFormat, timeSpan.Minutes, timeSpan.Seconds);
    }

    public override void SetIdleTransform(Transform tran)
    {
        if (transIdle == null) transIdle = tran;
    }

    #region Skin

    public void EquipSkinCollector(SkinRobotId id)
    {
        for (int i = 0; i < skinCollectors.Count; i++)
        {
            if (skinCollectors[i].skinRobotId == id)
            {
                skinCollectors[i].gameObject.SetActive(true);
                animator = skinCollectors[i].anim;
                collectorEquipment = skinCollectors[i];
            }
            else
            {
                skinCollectors[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion
}
