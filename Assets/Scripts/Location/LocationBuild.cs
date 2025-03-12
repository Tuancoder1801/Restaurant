using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationBuild : LocationBase
{
    public SpriteRenderer fillRenderer;
    public SpriteRenderer icon;
    public TextMeshPro textPrice;

    private Material material;
    private Transform tranPlayer;
    private IEnumerator ieWaitTakeMoney;
    private Location locationData;

    private void OnEnable()
    {
        material = fillRenderer.material;
    }

    private void OnDisable()
    {
        Clear();
    }

    public void SetData(Location location)
    {
        locationData = location;
        icon.sprite = location.icon;
        textPrice.text = location.price.ToString();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(StaticValue.CHARACTER_NAME_TAG))
        {
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
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            fill = Mathf.Lerp(0f, max, elapsed / duration);
            material.SetFloat("_Arc2", 360f - ((fill / max) * 360f));
            fillRenderer.material = material;

            elapsed += Time.deltaTime;
            yield return null;
        }

        material.SetFloat("_Arc2", 0f);
        fillRenderer.material = material;

        GameManager.Instance.OnBuildCompleted(this);
    }

    private void Clear()
    {
        material.SetFloat("_Arc2", 360f);
        fillRenderer.material = material;
    }

    public void SetMoney(double m)
    {
        //icon.gameObject.SetActive(false);
        //if (goResult != null)
        //{
        //    LocationBase lbase = goResult.GetComponent<LocationBase>();
        //    if (lbase != null && lbase.sprIcon != null)
        //    {
        //        spriteIcon.sprite = lbase.sprIcon;
        //        spriteIcon.gameObject.SetActive(true);
        //    }
        //}

        //goMoney.SetActive(true);

        //money = m;
        //txtMoney.text = VKCommon.ConvertStringMoney(money, ".");
    }
}
