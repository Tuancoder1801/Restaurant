using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Plate : MonoBehaviour
{
    public int maxStackNumber = 4;
    public List<Transform> itemsTransform;

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;
    public int currentStackNumber;

    public void Initialize(int stackNumber)
    {
        maxStackNumber = stackNumber;
        currentStackNumber = 0;
    }

    public bool IsExcessStackNumber()
    {
        return currentStackNumber >= maxStackNumber;
    }

    public void ReceiveItems(Transform item)
    {
        if (IsExcessStackNumber()) return;
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

    private void OnTriggerStay(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItems(player));
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
            }
        }
    }

    private IEnumerator SpawnItems(Player player)
    {
        while (isColliding && currentStackNumber > 0 && player.currentItemNumber < player.maxStackNumber)
        {
            BaseItem item = itemsTransform[currentStackNumber - 1].GetChild(0).GetComponent<BaseItem>();
            player.ReceiveItems(item);
            currentStackNumber--;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
