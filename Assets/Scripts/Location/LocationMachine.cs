using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LocationMachine : LocationBase
{
    public BaseItem item;
    public List<ItemPosition> materials;
    public ItemPosition product;

    public Animator machineAnim;
    public UILocation uiLocation;

    public GameObject goMakingProgress;
    public Image imgMakingProgress;

    public List<GameObject> goMaterials;

    public Transform posMachine;
    public Transform posChef;
    public bool keepMaterial;

    public float timeMaking;
    public bool isNeedChef = false;

    public Transform posProductCenter;
    public Transform posMaterialCenter;

    public GameObject goFxWork;

    private Coroutine ieWaitMakeProduct;
    public List<GameObject> chefs;

    protected float timeMakingCurrent;

    private void Start()
    {
        chefs = new List<GameObject>();

        materials.ForEach(x => x.Init());
        product.Init();

        if (uiLocation != null && materials != null)
        {
            uiLocation.LoadItem(materials);
        }

        if (timeMakingCurrent <= 0 && timeMaking > 0) timeMakingCurrent = timeMaking;
    }

    protected void OnEnable()
    {
        if (goMaterials != null) goMaterials.ForEach(x => x.SetActive(true));

        MakeProduct();
    }

    void OnDisable()
    {
        Clear();
    }

    public void AddChef(GameObject obj)
    {
        if (isNeedChef)
        {
            if (chefs == null) chefs = new List<GameObject>();

            chefs.Add(obj);

            MakeProduct();
        }
    }

    public void RemoveChef(GameObject obj)
    {
        if (isNeedChef)
        {
            chefs.Remove(obj);
        }
    }

    public override int MaxProductCanMake()
    {
        if (materials == null || materials.Count == 0) return 999;

        if (materials != null && materials.Count > 0)
        {
            int count = 9999;
            foreach (var material in materials)
            {
                var c = material.CountItem();
                if (c < count) count = c;
                if (count == 0) break;
            }

            return count + (ieWaitMakeProduct != null ? 1 : 0);
        }

        return 0;
    }

    public override BaseItem PopItem()
    {
        var item = product.PopItem();
        if (item != null) MakeProduct();
        return item;
    }

    public override void PushItem(BaseItem item)
    {
        if (materials != null && materials.Count > 0)
        {
            var material = materials.FirstOrDefault(x => x.itemId == item.itemId);

            if (material != null)
            {
                int index = material.GetIndexEmpty();
                material.PushItem(item, index);
                if (uiLocation != null) uiLocation.SetNumber(material.itemId, material.CountItem(), material.currentStackNumber);

                Sequence sequence = DOTween.Sequence();
                sequence.Append(
                item.transform.DOJump(material.itemPositions[index].position, 2f, 1, 0.5f).OnComplete(() =>
                {
                    item.transform.SetParent(material.itemPositions[index]);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localRotation = Quaternion.identity;
                    item.transform.localScale = Vector3.one;
                    MakeProduct();
                }));
            }
        }
    }

    public override List<ItemId> GetNeedItems()
    {
        if (materials != null && materials.Count > 0)
        {
            List<ItemId> needItems = new List<ItemId>();
            foreach (var material in materials)
            {
                if (!material.IsFullStack())
                {
                    needItems.Add(material.itemId);
                }
            }
            return needItems;
        }
        return null;
    }

    public ItemPosition GetItemMaterial(ItemId itemId)
    {
        return materials.FirstOrDefault(x => x.itemId == itemId);
    }

    public override Vector3 GetPosRawBin()
    {
        if (posMaterialCenter != null) return posMaterialCenter.position;
        return transform.position;
    }

    public override ItemId GetProductId()
    {
        return product.itemId;
    }

    public Tuple<ItemId, int> GetLocationRequire()
    {
        var itemPos = materials.OrderByDescending(m => m.GetNumberSlotBlank()).FirstOrDefault();
        return itemPos != null ? Tuple.Create(itemPos.itemId, itemPos.GetNumberSlotBlank()) : Tuple.Create(ItemId.None, 0);
    }

    private void PlayMachineAnim(string anim)
    {
        if (machineAnim != null) machineAnim.SetTrigger(anim);
    }

    private void MakeProduct()
    {
        if (ieWaitMakeProduct == null)
        {
            ieWaitMakeProduct = StartCoroutine(IEWaitMakeProduct());
        }
    }

    private IEnumerator IEWaitMakeProduct()
    {
        yield return new WaitForEndOfFrame();

        AIChef chef = null;
        List<BaseItem> items = new List<BaseItem>();
        bool isAnim = false;

        while (!product.IsFullStack() && (!isNeedChef || chefs.Count > 0) && (materials.Count <= 0 || materials.All(x => x.IsHasItem())))
        {
            yield return new WaitForEndOfFrame();

            if (materials.Count > 0)
            {
                foreach (var material in materials)
                {
                    var im = material.PopItem();

                    if (uiLocation != null) uiLocation.SetNumber(material.itemId, material.CountItem(), material.currentStackNumber);

                    if (im != null)
                    {
                        if (keepMaterial) items.Add(im);

                        DG.Tweening.Sequence sequence = DOTween.Sequence();
                        sequence.Append(
                        im.transform.DOJump(posMachine.position, 1f, 1, 0.2f).OnComplete(() =>
                        {
                            im.transform.SetParent(posMachine);
                            im.transform.localPosition = Vector3.zero;
                            im.transform.localRotation = Quaternion.identity;
                            im.transform.localScale = Vector3.one;
                        }));
                    }
                }
            }

            if (!isAnim)
            {
                isAnim = true;
                PlayMachineAnim("work");
                if (goFxWork != null) goFxWork.SetActive(true);

                if (isNeedChef && chefs.Count > 0)
                {
                    if (chef == null)
                    {
                        foreach (var c in chefs)
                        {
                            try
                            {
                                chef = c.GetComponent<AIChef>();
                            }
                            catch { }

                            if (chef != null) break;
                        }
                    }

                    if (chef != null) chef.PlayAnimCook();
                }
            }

            if (goMakingProgress != null)
            {
                goMakingProgress.SetActive(true);
                imgMakingProgress.fillAmount = 0f;

                float progress = 0;
                while (progress < timeMakingCurrent)
                {
                    progress += Time.deltaTime;
                    imgMakingProgress.fillAmount = progress / timeMakingCurrent;
                    yield return new WaitForEndOfFrame();
                }

                imgMakingProgress.fillAmount = 1f;
                goMakingProgress.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(timeMakingCurrent);
            }

            items.Clear();
            MakeProductDone();
        }

        if (isAnim)
        {
            PlayMachineAnim("idle");
            if (goFxWork != null) goFxWork.SetActive(false);
            if (isNeedChef && chefs.Count > 0)
            {
                if (chef != null) chef.StopAnimCook();
            }
        }

        ieWaitMakeProduct = null;
    }

    private void MakeProductDone()
    {
        BaseItem i = Instantiate(item, posMachine.position, posMachine.rotation);
        int index = product.GetIndexEmpty();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(
        i.transform.DOMove(product.itemPositions[index].position, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            i.transform.SetParent(product.itemPositions[index]);
            i.transform.localPosition = Vector3.zero;
            i.transform.localRotation = Quaternion.identity;
            i.transform.localScale = Vector3.one;
        }));

        product.PushItem(i, index);
    }

    private void Clear()
    {
        StopCoroutine(ieWaitMakeProduct);
    }
}
