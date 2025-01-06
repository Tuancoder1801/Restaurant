using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Plate : MonoBehaviour
{
    public int maxStackNumber = 6;
    public List<Transform> itemsTransform;

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;
    public int currentStackNumber;

    public void ReceiveItems(Transform item)
    {
        ItemId itemId = item.GetComponent<BaseItem>().itemId;
        Sequence sequence = DOTween.Sequence();


        sequence.Append(
        item.DOJump(itemsTransform[currentStackNumber].position, 2f, 1, 0.5f).OnComplete(() =>
        {
            item.SetParent(itemsTransform[currentStackNumber]);
            item.localPosition = Vector3.zero;
            item.localRotation = Quaternion.identity;
            item.localScale = Vector3.one;

            currentStackNumber++;
        })
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null && !isColliding)
        {
            isColliding = true;

            if (itemSpawnCoroutine == null)
            {
                itemSpawnCoroutine = StartCoroutine(SpawnItems(player));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();

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

    private IEnumerator SpawnItems(Player player)
    {
        while (currentStackNumber > 0 && player.currentItemNumber < player.maxStackNumber)
        {
            BaseItem item = itemsTransform[currentStackNumber - 1].GetChild(0).GetComponent<BaseItem>();
            if (item == null)
            {
                Debug.LogError("BaseItem component is missing on " + itemsTransform[currentStackNumber - 1].name);
                break;
            }
            player.ReceiveItems(item);
            currentStackNumber--;
            yield return new WaitForSeconds(0.5f);
        }

        itemSpawnCoroutine = null;
    }
}
