using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawBin : MonoBehaviour
{
    public BaseItem baseItem;
    public Transform itemIndex;

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnCollisionEnter: {other.name}");
        Player player = other.GetComponentInParent<Player>();

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
        Player player = other.GetComponentInParent<Player>();

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
        for (int i = 0; i < player.maxStackNumber; i++)
        {   
            if(player.currentItemNumber < player.maxStackNumber)
            {
                BaseItem item = Instantiate(baseItem, itemIndex.position, itemIndex.rotation);
                player.ReceiveItems(item);
            }

            yield return new WaitForSeconds(0.5f);
        }

        itemSpawnCoroutine = null;
    }
}
