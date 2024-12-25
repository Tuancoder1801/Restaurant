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
    //public Dictionary<ItemId, int> currentItemPos = new Dictionary<ItemId, int>();

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;

    private void Awake()
    {
        /*for (int i = 0; i < itemsPosition.Count; i++)
        {
            ItemId itemId = itemsPosition[i].itemId;

            if (!currentItemPos.ContainsKey(itemId))
            {
                currentItemPos[itemId] = 0;
            }
        }*/
    }

    public bool HasItem()
    {
        int validItemPositionCount = 0;

        for (int i = 0; i < itemsPosition.Count; i++)
        {
            bool hasItemInThisPosition = false;
            List<Transform> pos = itemsPosition[i].itemPositions;

            for (int j = 0; j < pos.Count; j++)
            {
                if (pos[j].childCount > 0)
                {
                    hasItemInThisPosition = true;
                    break;
                }
            }

            if (hasItemInThisPosition)
            {
                validItemPositionCount++;
            }

            if (validItemPositionCount >= 2)
            {
                return true;
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