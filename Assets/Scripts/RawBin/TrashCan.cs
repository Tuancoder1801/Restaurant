using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    public Transform dropItemIndex;

    private bool isColliding = false;
    private Coroutine itemSpawnCoroutine;

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.transform.root.GetComponent<Player>();

        if (player != null)
        {
            isColliding = true;

            if (itemSpawnCoroutine == null)
            {
                itemSpawnCoroutine = StartCoroutine(SpawItems(player));
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Player player = collision.transform.root.GetComponent<Player>();

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
        for (int i = 0; i < player.maxStackNumber; i++)
        {
            if (player.currentItemNumber <= 0)
            {
                break;
            }

            player.DropItems(dropItemIndex);
            yield return new WaitForSeconds(0.5f);
        }

        itemSpawnCoroutine = null;
    }
}
