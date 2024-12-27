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

    private void Awake()
    {
    }

    public bool HasItem()
    {
        for(int i = 0; i < itemsPosition.Count; i++)
        {
            List<Transform> items = itemsPosition[i].itemPositions;

            for(int j =  0; j < items.Count; j++)
            {
                if (items[j].childCount > 0)
                {
                    return true;
                }
            }
        }


        return false;
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