using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using static UnityEditor.Progress;
using static UnityEditor.FilePathAttribute;

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

    private void OnTriggerStay(Collider other)
    {
        Character character = other.transform.root.GetComponent<Character>();

        if (character != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItemsCoroutine(character));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.transform.root.GetComponent<Character>();

        if (character != null)
        {
            isColliding = false;
            if (itemSpawnCoroutine != null)
            {
                StopCoroutine(itemSpawnCoroutine);
            }
        }
    }

    private IEnumerator SpawnItemsCoroutine(Character character)
    {
        while (isColliding)
        {
            character.ReleaseItems(itemsPosition);
            yield return new WaitForSeconds(0.5f);
        }
    }
}