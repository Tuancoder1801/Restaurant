﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public Transform dropItemIndex;

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;

    private void OnTriggerStay(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawItems(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player != null)
        {
            isColliding = false;

            if (itemSpawnCoroutine != null)
            {
                StopCoroutine(itemSpawnCoroutine);
            }
        }
    }

    private IEnumerator SpawItems(Player player)
    {
        while(isColliding)
        {         
            player.DropItems(dropItemIndex);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
