using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class LocationBuild : LocationBase
{
    public BaseItem moneyItem;
    public SpriteRenderer fillRenderer;
    public SpriteRenderer icon;
    public TextMeshPro textPrice;
    public Transform indexMoneyDrop;

    private Material material;
    private Transform tranPlayer;
    private IEnumerator ieWaitTakeMoney;
    private Location locationData;

    public double money;
    private double moneyPer;
    private bool isWarning;


    private void OnEnable()
    {
        material = fillRenderer.material;
    }

    private void OnDisable()
    {
        Clear();
    }

    private void Update()
    {
        money = double.Parse(textPrice.text);
    }

    public void SetData(Location location)
    {
        locationData = location;
        icon.sprite = location.icon;
        textPrice.text = VKCommon.ConvertStringMoney(location.price, ".");
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {   
            isWarning = false;
            tranPlayer = other.transform;
            if (ieWaitTakeMoney != null) StopCoroutine(ieWaitTakeMoney);
            ieWaitTakeMoney = LoadFill(); // Gán coroutine
            StartCoroutine(ieWaitTakeMoney);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {   
            isWarning = false;
            tranPlayer = null;
            material.SetFloat("_Arc2", 360f);
            fillRenderer.material = material;

            if (ieWaitTakeMoney != null) StopCoroutine(ieWaitTakeMoney);
        }
    }

    private IEnumerator LoadFill()
    {
        float fill = 0f;
        float max = 1f;

        while (true)
        {
            fill += Time.deltaTime;
            material.SetFloat("_Arc2", 360f - ((fill / max) * 360f));
            fillRenderer.material = material;

            if (fill >= max) break; 
            yield return new WaitForEndOfFrame();
        }

        moneyPer = Mathf.Ceil((float)money * 0.01f);
        while (money > 0) 
        {
            TakeMoney();
            yield return new WaitForSeconds(0.05f);
        }

        ieWaitTakeMoney = null;
        //GameManager.Instance.OnBuildCompleted(this);
    }

    private void Clear()
    {
        material.SetFloat("_Arc2", 360f);
        fillRenderer.material = material;
    }

    private void TakeMoney()
    {
        if (money <= 0)
        {
            StopCoroutine(ieWaitTakeMoney);
            return;
        }

        if (UIGame.Instance.currentMoney <= 0)
        {
            if (!isWarning)
            {
                isWarning = true;
            }
            return;
        }

        double mPer = UIGame.Instance.currentMoney > moneyPer ? moneyPer : UIGame.Instance.currentMoney;
        if (money < mPer) mPer = money;

        if (!GameManager.Instance.SpendMoney(-mPer, true)) 
        {
            return;
        }

        isWarning = false;

        money -= mPer;

        if (money < 0) textPrice.text = "";
        else textPrice.text = VKCommon.ConvertStringMoney(money, ".");

        BaseItem iMoney = Instantiate(moneyItem, tranPlayer.position, tranPlayer.rotation, tranPlayer);

        bool isUnlock = money <= 0;

        Tween jumpTween = iMoney.transform.DOJump(transform.position, 2f, 1, 0.5f);

        jumpTween.OnComplete(() =>
        {
            Destroy(iMoney.gameObject);

            if (isUnlock)
            {
                GameManager.Instance.OnBuildCompleted(this);
            }
        });
    }
}
