using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationMachine : LocationBase
{
    public List<ItemPosition> materials;
    public ItemPosition product;

    public Animator machineAnim;
    public UILocation uiLocation;

    public GameObject goMakingProgress;
    public Image imgMakingProgress;

    public List<GameObject> goMaterials;

    public Transform posMachine;
    public Transform posChef;
    public bool keepMaterial;

    public float timeMaking;
    public bool isNeedChef = false;

    public Transform posProductCenter;
    public Transform posMaterialCenter;

    private IEnumerator  ieWaitMakeProduct;
    private  List<GameObject> chefs;

    protected float timeMakingCurrent;

    private void Start()
    {   
            
    }

    protected void OnEnable()
    {
        if (goMaterials != null) goMaterials.ForEach(x => x.SetActive(true));
    }

    void OnDisable()
    {
        //Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }


}
