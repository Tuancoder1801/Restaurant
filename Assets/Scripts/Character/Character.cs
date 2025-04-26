using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{   
    public HumanId humanId;

    public Animator animator;

    public List<Transform> positions;
    public List<BaseItem> items;
    public List<int> indexStarts;
    public int currentMax;

    public float speedBase;
    public float speed;

    protected List<LocationBase> locations = new List<LocationBase>();
    protected float timeCount = 0.2f;
    protected string currentAnim;

    protected virtual void Start()
    {
        if(items == null) items = new List<BaseItem>();
        if(items.Count < positions.Count)
        {
            for(int i = items.Count; i < positions.Count; i++)
            {
                items.Add(null);
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StaticValue.LOCATION_NAME_TAG))
        {
            if (locations.Count <= 0) timeCount = 0;
            locations.Add(other.GetComponent<LocationBase>());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StaticValue.LOCATION_NAME_TAG))
        {
            locations.Remove(other.GetComponent<LocationBase>());
        }
    }

    #region Anim
    protected virtual void PlayAnimMove()
    {
        if (IsEmpty())
            PlayAnim(StaticValue.ANIM_TRIGGER_WALK);
        else       
            PlayAnim(StaticValue.ANIM_TRIGGER_WALK_HOLD);
    }

    protected virtual void PlayAnimIdle()
    {
        if (IsEmpty())
            PlayAnim(StaticValue.ANIM_TRIGGER_IDLE);
        else
            PlayAnim(StaticValue.ANIM_TRIGGER_IDLE_HOLD);
    }

    protected virtual void PlayAnim(string anim)
    {
        if(animator == null) return;
        if(currentAnim == anim)
        {
            return;
        }
        if (!string.IsNullOrEmpty(currentAnim)) animator.ResetTrigger(currentAnim);

        currentAnim = anim;
        animator.SetTrigger(anim);
    }

    protected void PlayLastAnim()
    {
        if (!string.IsNullOrEmpty(currentAnim))
        {
            animator.SetTrigger(currentAnim);
        }
    }

    #endregion

    #region Item

    public virtual void PushItem(BaseItem item, Action callBack = null)
    {
        if(item != null)
        {
            int index = GetIndexEmpty();
            if(index >= 0)
            {
                PushItem(item, index);

                DG.Tweening.Sequence sequence = DOTween.Sequence();
                sequence.Append(
                item.transform.DOJump(positions[index].position, 1f, 1, 0.2f).OnComplete(() =>
                {
                    item.transform.SetParent(positions[index]);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localRotation = Quaternion.identity;
                    item.transform.localScale = Vector3.one;

                    callBack?.Invoke();
                }));
                ReloadItemPos();
            }
            else
            {
                callBack?.Invoke();
            }
        }
        else
        {
            callBack?.Invoke();
        }
    }

    #endregion

    public virtual void SetIdleTransform(Transform tran) 
    {

    }

    public bool IsFullStack()
    {
        return items.Count(x => x != null) >= currentMax;
    }

    public bool IsEmpty()
    {
        return items.Count(x => x != null) == 0;
    }

    public int GetIndexEmpty()
    {
        return items.FindIndex(x => x == null);
    }

    public void PushItem(BaseItem item, int index)
    {
        items[index] = item;
    }

    public virtual BaseItem PopItem()
    {
        int index = items.FindLastIndex(x => x != null);
        if(index >= 0)
        {
            var item = items[index];
            items[index] = null;
            ReloadItemPos();

            return item;
        }
        return null;
    }

    public virtual BaseItem PopItem(ItemId itemId)
    {
        int index = items.FindLastIndex(x => x != null && x.itemId == itemId);
        if (index >= 0)
        {
            var item = items[index];
            items[index] = null;

            SortItem();

            return item;
        }
        return null;
    }

    private void SortItem()
    {
        int indexNull = -1;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                if (indexNull < 0) indexNull = i;
            }
            else
            {
                if (indexNull >= 0)
                {
                    Vector3 vTemp = positions[i].localPosition;
                    positions[i].localPosition = positions[indexNull].localPosition;
                    positions[indexNull].localPosition = vTemp;

                    var temp = positions[i];
                    positions[i] = positions[indexNull];
                    positions[indexNull] = temp;

                    items[indexNull] = items[i];
                    items[i] = null;

                    indexNull = i;
                    for (int j = 0; j <= i; j++)
                    {
                        if (items[j] == null)
                        {
                            indexNull = j;
                            break;
                        }
                    }
                }
            }
        }
        ReloadItemPos();
    }

    private void ReloadItemPos()
    {
        float pos = 0f;
        for(int i = 0; i < positions.Count; i++)
        {
            if (indexStarts.Contains(i))
            {
                pos = positions[i].transform.localPosition.y;
                positions[i].transform.localPosition = new Vector3(positions[i].transform.localPosition.x, pos, positions[i].transform.localPosition.z);
            }
            else
            {
                positions[i].transform.localPosition = new Vector3(positions[i].transform.localPosition.x, pos, positions[i].transform.localPosition.z);
            }
            if (items[i] != null && items[i].height > 0)
            {
                pos += items[i].height;
            }
            else
            {
                pos += 0.35f;
            }
        }
    }
}
