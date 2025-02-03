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

        // Tăng trước currentStackNumber để đảm bảo trạng thái Plate được cập nhật
        int previousStackNumber = currentStackNumber; // Lưu lại trạng thái để rollback nếu cần
        currentStackNumber++;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            item.DOJump(itemsTransform[previousStackNumber].position, 0.5f, 1, 0.2f)
                .OnComplete(() =>
                {
                    // Khi hoàn tất, đưa item vào vị trí chính xác
                    item.SetParent(itemsTransform[previousStackNumber]);
                    item.localPosition = Vector3.zero;
                    item.localRotation = Quaternion.identity;
                    item.localScale = Vector3.one;
                })
        )
        .OnKill(() =>
        {
            // Rollback nếu hoạt ảnh bị huỷ hoặc không hoàn thành
            currentStackNumber = previousStackNumber;
            Debug.LogWarning("Hoạt ảnh bị huỷ, rollback số lượng trên Plate.");
        });
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
            yield return new WaitForSeconds(0.3f);
        }
    }
}
