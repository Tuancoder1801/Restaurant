using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTrash : LocationBase
{
    public Transform tran;

    public override void PushItem(BaseItem item)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(
        item.transform.DOJump(tran.position, 2f, 1, 0.5f).OnComplete(() =>
        {
            Destroy(item.gameObject);
        }));
    }
}
