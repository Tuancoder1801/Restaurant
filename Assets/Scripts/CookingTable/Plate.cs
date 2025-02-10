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

    public bool CanAddItem()
    {
        return currentStackNumber < maxStackNumber;
    }

    public void ReceiveItems(Transform item)
    {
        if (!CanAddItem()) return;

        int previousStackNumber = currentStackNumber;
        currentStackNumber++;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            item.DOJump(itemsTransform[previousStackNumber].position, 0.5f, 1, 0.2f)
                .OnComplete(() =>
                {
                    item.SetParent(itemsTransform[previousStackNumber]);
                    item.localPosition = Vector3.zero;
                    item.localRotation = Quaternion.identity;
                    item.localScale = Vector3.one;
                })
        )
        .OnKill(() =>
        {
            currentStackNumber = previousStackNumber;         
        });
    }

    private void OnTriggerStay(Collider other)
    {
        Character character = other.GetComponent<Character>();

        if (character != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItems(character));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponent<Character>();

        if (character != null)
        {
            isColliding = false;

            if (itemSpawnCoroutine != null)
            {
                StopCoroutine(itemSpawnCoroutine);
            }
        }
    }

    private IEnumerator SpawnItems(Character character)
    {
        while (isColliding && currentStackNumber > 0 && character.currentItemNumber < character.maxStackNumber)
        {
            if (currentStackNumber - 1 >= 0 && currentStackNumber - 1 < itemsTransform.Count)
            {
                // Kiểm tra xem có con nào trong itemsTransform[currentStackNumber - 1]
                if (itemsTransform[currentStackNumber - 1].childCount > 0)
                {
                    // Lấy item từ vị trí hiện tại trong itemsTransform
                    BaseItem item = itemsTransform[currentStackNumber - 1].GetChild(0).GetComponent<BaseItem>();
                    if (item != null)
                    {
                        character.ReceiveItems(item);
                        currentStackNumber--;
                    }
                }
                else
                {
                    Debug.LogWarning("No item to spawn in stack position " + (currentStackNumber - 1));
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
