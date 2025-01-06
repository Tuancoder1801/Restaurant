using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using static UnityEditor.Progress;

public class Tray : MonoBehaviour
{
    public int maxStackNumber = 4;
    public List<ItemPosition> itemsPosition = new List<ItemPosition>();

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;

    public bool HasItem()
    {
        foreach (var positionList in itemsPosition)
        {
            List<Transform> items = positionList.itemPositions;

            bool hasItemInList = false;

            foreach (var position in items)
            {
                if (position.childCount > 0)
                {
                    hasItemInList = true;
                    break;
                }
            }

            if (!hasItemInList)
            {
                return false;
            }
        }

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.transform.root.GetComponent<Player>();

        if (player != null && !isColliding)
        {
            isColliding = true;

            if (itemSpawnCoroutine == null)
            {
                itemSpawnCoroutine = StartCoroutine(SpawItems(player));
            }
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
                itemSpawnCoroutine = null;
            }
        }
    }

    private IEnumerator SpawItems(Player player)
    {
        for (int i = 0; i < maxStackNumber; i++)
        {   
            player.ReleaseItems(itemsPosition, maxStackNumber);
            yield return new WaitForSeconds(0.5f);
        }
        itemSpawnCoroutine = null;
    }
}