using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawBin : MonoBehaviour
{
    public BaseItem baseItem;
    public Transform itemIndex;
    public Transform pickIndex;

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;

    private void OnTriggerStay(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();

        if (character != null && !isColliding)
        {
            isColliding = true;
            itemSpawnCoroutine = StartCoroutine(SpawnItems(character));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = other.GetComponentInParent<Character>();

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
        while (isColliding)
        {
            if (character.currentItemNumber < character.maxStackNumber)
            {
                BaseItem item = Instantiate(baseItem, itemIndex.position, itemIndex.rotation);
                character.ReceiveItems(item);
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
}
