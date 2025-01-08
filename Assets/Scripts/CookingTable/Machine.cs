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

    public ParticleSystem processing;

    private bool isCooking = false;
    private bool hasIngredient = false;

    private void Awake()
    {
        timer.gameObject.SetActive(false);
        processing.gameObject.SetActive(false);
    }

    private void StartCooking()
    {
        if (!tray.HasItem())
        {
            return;
        }

        if(plate.IsExcessStackNumber())
        {
            return;
        }

        if (!isCooking)
        {
            isCooking = true; 
            hasIngredient = false;

            timer.StartCount();
            processing.gameObject.SetActive(true);
            processing.Play();
            StartCoroutine(CheckCookingProgress());
        }
    }

    private IEnumerator CheckCookingProgress()
    {
        while(isCooking)
        {
            if(!hasIngredient)
            {
                AddIngredientsToCook();
            }

            if(timer.IsCookCompleted())
            {
                CookingCompleted();
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
        timer.StopCount();
    }

    private void AddIngredientsToCook()
    {
        hasIngredient = true;

        Sequence sequence = DOTween.Sequence();
        List<Transform> itemsToAdd = new List<Transform>();

        for(int i = 0; i < tray.itemsPosition.Count; i++)
        {
            ItemId itemId = tray.itemsPosition[i].itemId;
            ItemPosition itemPos = tray.itemsPosition[i];

            if (tray.HasItem() && itemPos.itemPositions.Count > 0)
            {
                for(int j = itemPos.itemPositions.Count - 1; j >= 0; j--)
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

        for(int i = 0; i < itemsToAdd.Count; i++)
        {   
            Transform item = itemsToAdd[i];

            item.DOMove(cookIndex.position, 0.5f).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    Destroy(item.gameObject);
                });
        }
    }

    private void TakeOutTheFood()
    {
        BaseItem item = Instantiate(baseItem, cookIndex.position, cookIndex.rotation);
        plate.ReceiveItems(item.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.transform.root.GetComponent<Player>();

        if(player != null)
        {
            StartCooking();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Player player = other.transform.root.GetComponent<Player>();

        if (player != null)
        {
            if(!timer.isActiveAndEnabled)
            {
                StartCooking();
            }
        }
    }
}
