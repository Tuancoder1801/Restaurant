using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Machine : MonoBehaviour
{
    public Timer timer;
    public Transform cookIndex;
    public Tray tray;
    public Plate plate;
    public BaseItem baseItem;
    public bool isCooking = false;

    public ParticleSystem processing;
    private bool hasIngredient = false;

    private void Awake()
    {
        timer.gameObject.SetActive(false);
        processing.gameObject.SetActive(false);
    }

    private void StartCooking()
    {
        if (!tray.HasItem()) return;
        if (!plate.CanAddItem()) return;

        if (!isCooking)
        {
            isCooking = true;
            hasIngredient = false;

            timer.StartCount();
            processing.gameObject.SetActive(true);
            processing.Play();
            StartCoroutine(CookingProgress());
        }
    }

    private IEnumerator CookingProgress()
    {
        while (isCooking)
        {
            if (!plate.CanAddItem())
            {
                EndCooking();
                yield break;
            }

            if (!hasIngredient)
            {
                AddIngredientsToCook();
            }

            if (timer.IsCookCompleted())
            {
                CookingCompleted();

                if (!plate.CanAddItem()) yield break;
            }

            yield return null;
        }
    }

    private void CookingCompleted()
    {
        processing.gameObject.SetActive(false);
        processing.Stop();
        TakeOutTheFood();
        EndCooking();
    }

    private void EndCooking()
    {
        isCooking = false;
        timer.StopCount(); // Dừng timer
        timer.gameObject.SetActive(false); // Ẩn timer
        processing.gameObject.SetActive(false);
    }

    /*private void AddIngredientsToCook()
    {
        if (!plate.IsExcessStackNumber()) // Nếu Plate đầy, không thêm nguyên liệu
        {
            return;
        }

        hasIngredient = true;

        Sequence sequence = DOTween.Sequence();
        List<Transform> itemsToAdd = new List<Transform>();

        for (int i = 0; i < tray.itemsPosition.Count; i++)
        {
            ItemId itemId = tray.itemsPosition[i].itemId;
            ItemPosition itemPos = tray.itemsPosition[i];

            if (tray.HasItem() && itemPos.itemPositions.Count > 0)
            {
                for (int j = itemPos.itemPositions.Count - 1; j >= 0; j--)
                {
                    Transform position = itemPos.itemPositions[j];

                    if (position.childCount > 0)
                    {
                        Transform item = position.GetChild(0);
                        itemsToAdd.Add(item);

                        itemPos.currentStackNumber--;
                        break;
                    }
                }
            }
        }

        if (!plate.IsExcessStackNumber())
        {
            return;
        }

        for (int i = 0; i < itemsToAdd.Count; i++)
        {
            Transform item = itemsToAdd[i];

            item.DOMove(cookIndex.position, 0.2f).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    Destroy(item.gameObject);
                });
        }
    }*/

    private void AddIngredientsToCook()
    {
        hasIngredient = true;

        foreach (var itemPos in tray.itemsPosition)
        {
            if (itemPos.itemPositions.Count == 0) continue;

            for (int j = itemPos.itemPositions.Count - 1; j >= 0; j--)
            {
                Transform position = itemPos.itemPositions[j];
                if (position.childCount > 0)
                {
                    Transform item = position.GetChild(0);

                    itemPos.currentStackNumber--;

                    item.DOMove(cookIndex.position, 0.2f).SetEase(Ease.InOutQuad)
                        .OnComplete(() =>
                        {
                            Destroy(item.gameObject);
                        });

                    break;
                }
            }
        }
    }

    private void TakeOutTheFood()
    {
        BaseItem item = Instantiate(baseItem, cookIndex.position, cookIndex.rotation);
        plate.ReceiveItems(item.transform);
    }

    private void OnTriggerStay(Collider other)
    {
        Player player = other.transform.root.GetComponent<Player>();
        AIChef chef = other.GetComponent<AIChef>();

        if (player != null)
        {
            if (!timer.isActiveAndEnabled)
            {
                StartCooking();
            }
        }

        if (chef != null)
        {
            chef.machine = this;

            if (!timer.isActiveAndEnabled)
            {
                StartCooking();
            }
        }
    }
}
