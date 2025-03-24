using System;
using System.Collections;
using System.Collections.Generic;
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

    private List<SubLocationMoney> locationMoneys;
    private List<Transform> wayPoints;
    private SubLocationMoney locationCurrent;

    [HideInInspector]
    public float timeCurrent;
    private float timeTemp;
    private TimeSpan timeSpan;
    private string strTimeFormat = "{0:00}m {1:00}s";


}
