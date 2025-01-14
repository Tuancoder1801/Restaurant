using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using static UnityEditor.Progress;

public class Tray : MonoBehaviour
{
    public List<ItemPosition> itemsPosition = new List<ItemPosition>();

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;

    public bool HasItem()
    {
        foreach (var positionList in itemsPosition)
        {
            bool hasItemInList = false;

            foreach (var position in positionList.itemPositions)
            {
                if (position.childCount > 0)
                {
                    hasItemInList = true;
                    break;
                }
            }

            if (!hasItemInList) return false;
        }

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.transform.root.GetComponent<Player>();

        if (player != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItemsCoroutine(player));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Player player = other.transform.root.GetComponent<Player>();

        if (player != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItemsCoroutine(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.transform.root.GetComponent<Player>();

        if (player != null)
        {
            isColliding = false;
            if (itemSpawnCoroutine != null)
            {
                StopCoroutine(itemSpawnCoroutine);
            }
        }
    }

    private IEnumerator SpawnItemsCoroutine(Player player)
    {
        while (isColliding)
        {            
            player.ReleaseItems(itemsPosition);
            yield return new WaitForSeconds(0.5f);
        }
    }
}