using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public ItemId itemId;
    public float height;

    private bool isMoving;

    public void MoveNormal(Transform tranParent, Vector3 vFrom, Vector3 vTo, Vector3 rTo, float time, Action callback, LeanTweenType moveType = LeanTweenType.easeInSine)
    {
        if (isMoving) LeanTween.cancel(gameObject);
        isMoving = true;
        transform.SetParent(tranParent);
        transform.position = vFrom;

        gameObject.SetActive(true);

        Vector3 localFrom = transform.localPosition;
        Vector3 scaleFrom = transform.localScale;
        Vector3 eulerFrom = transform.localEulerAngles;
        LeanTween.value(gameObject, (value) => {
            transform.localPosition = Vector3.Lerp(localFrom, vTo, value);
            transform.localScale = Vector3.Lerp(scaleFrom, Vector3.one, value);
            transform.localEulerAngles = Vector3.Lerp(eulerFrom, rTo, value);
        }, 0f, 1f, time).setEase(moveType).setOnComplete(() =>
        {
            transform.localPosition = vTo;
            transform.localScale = Vector3.one;
            transform.localEulerAngles = rTo;

            isMoving = false;
            if (callback != null) callback.Invoke();
        });
    }
}