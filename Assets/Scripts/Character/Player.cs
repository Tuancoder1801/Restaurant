﻿using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : Character
{
    public Joystick joystick;
    public CharacterController charactor;

    public float speedRotate;
    public float timeRequest;
    public float timeDelayUpdate;

    public List<PlayerEquipment> skinPlayers;
    public List<Glass> skinGlasses;

    public PlayerEquipment playerEquipment;
    public Glass glass;

    public AudioClip sfxDopItem;
    public AudioClip sfxPushItem;

    public bool isUI;

    protected void OnEnable()
    {
        timeDelayUpdate = 0.5f;
        joystick = UIGameManager.Instance.joystick;
        EquipSkinPlayer((SkinPlayerId)UserData.skin.GetEquippedSkin(SkinType.Set));
    }

    protected void Update()
    {
        if (isUI) return;
        UpdateStateNormal();
    }

    protected void FixedUpdate()
    {
        if (isUI) return;

        Move();
    }

    #region Item

    public override void PushItem(BaseItem item, Action callBack = null)
    {
        base.PushItem(item, callBack);
        ReplayAnim();
    }

    public override BaseItem PopItem(ItemId itemId)
    {
        var item = base.PopItem(itemId);
        if (item != null)
        {
            ReplayAnim();
        }

        return item;
    }

    #endregion

    #region Update

    protected void Move()
    {
        if (joystick.Vertical == 0 && joystick.Horizontal == 0)
        {
            if(currentAnim == null || !currentAnim.StartsWith("isIdle"))
            {
                PlayAnimIdle();
            }
        }
        else
        {
            if(currentAnim == null || !currentAnim.StartsWith("isWalk"))
            {
                PlayAnimMove();
            }

            UpdateMove(joystick.Vertical, joystick.Horizontal);
        }
    }

    private void UpdateMove(float vertical, float horizontal)
    {
        Vector3 direction = Vector3.forward * vertical + Vector3.right * horizontal;
        direction = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * direction;

        charactor.Move(direction * Time.deltaTime * speed);

        var targetRotation = Quaternion.LookRotation(direction);

        var tempVector3 = transform.position;
        tempVector3.y = 0;
        transform.SetPositionAndRotation(tempVector3, targetRotation);
    }

    private void UpdateStateNormal()
    {
        if(locations.Count > 0)
        {
            timeCount -= Time.deltaTime;
            if(timeCount <= 0)
            {
                timeCount = timeRequest;
                UpdateLocationAction();
            }
        }
    }

    #endregion

    #region Location

    protected void UpdateLocationAction()
    {
        bool isDelay = false;
        foreach (var location in locations) 
        {
            switch (location.locationId)
            {
                case LocationId.RawBin:
                    UpdateLocationRawbin(location);
                    break;
                case LocationId.Machine:
                    UpdateLocationMachine(location);
                    break;
                case LocationId.Trash:
                    isDelay = true;
                    UpdateLocationTrash(location);
                    break;
                case LocationId.Table:
                case LocationId.TableVip:
                    UpdateLocationTable(location);
                    break;
            }
        }
        if (!isDelay) timeDelayUpdate = 0.5f;
    }

    protected void UpdateLocationRawbin(LocationBase location)
    {
        PopItemFromMachine(location);
    }

    protected void UpdateLocationMachine(LocationBase location)
    {
        LocationMachine locationMachine = location as LocationMachine;
        if(locationMachine.posMaterialCenter == null || locationMachine.posProductCenter == null)
        {
            PushItemToMachine(location);
            PopItemFromMachine(location);
        }
        else
        {
            float distancePush = Vector3.Distance(this.transform.position, locationMachine.posMaterialCenter.position);
            float distancePop = Vector3.Distance(this.transform.position, locationMachine.posProductCenter.position);

            if (distancePush < distancePop)
            {
                locationMachine.goPlayer = null;
                PushItemToMachine(location);
            }
            else
            {
                locationMachine.goPlayer = gameObject;
                PopItemFromMachine(location);
            }
        }
    }

    protected void UpdateLocationTable(LocationBase location)
    {
        // thả nguyên liệu
        var needItems = location.GetNeedItems();
        if (needItems != null && needItems.Count > 0)
        {
            foreach (var itemType in needItems)
            {
                var item = PopItem(itemType);
                if (item != null)
                {   
                    AudioManager.Instance.audioSFX.PlayOneShot(sfxDopItem);

                    location.PushItem(item);
                }
            }
        }
    }

    protected void UpdateLocationTrash(LocationBase location)
    {
        timeDelayUpdate -= timeRequest;
        if (timeDelayUpdate > 0) return;

        var item = PopItem();
        ReplayAnim();
        if (item != null)
        {
            AudioManager.Instance.audioSFX.PlayOneShot(sfxDopItem);

            location.PushItem(item);
        }
    }

    private void PushItemToMachine(LocationBase location)
    {
        var needItems = location.GetNeedItems();
        if (needItems != null && needItems.Count > 0)
        {
            foreach (var itemType in needItems)
            {
                var item = PopItem(itemType);
                if (item != null)
                {
                    AudioManager.Instance.audioSFX.PlayOneShot(sfxDopItem);

                    location.PushItem(item);
                }
            }
        }
    }

    private void PopItemFromMachine(LocationBase location)
    {
        if (!IsFullStack())
        {
            var item = location.PopItem();

            if(item != null)
            {
                AudioManager.Instance.audioSFX.PlayOneShot(sfxPushItem);

                PushItem(item);
            }
        }
    }

    #endregion

    private void ReplayAnim()
    {
        if(currentAnim.StartsWith("isIdle")) PlayAnimIdle();
        else if(currentAnim.StartsWith("isWalk")) PlayAnimMove();
    }

    #region Skin

    public void EquipSkinPlayer(SkinPlayerId id)
    {
        for(int i = 0; i < skinPlayers.Count; i++)
        {   
            if(skinPlayers[i].id == id)
            {
                skinPlayers[i].gameObject.SetActive(true);
                animator = skinPlayers[i].animator;
                playerEquipment = skinPlayers[i];
            }
            else
            {
                skinPlayers[i].gameObject.SetActive(false);
            }
        } 
    }

    public void EquipSkinGlass(SkinGlassesId id)
    {
        if (id != SkinGlassesId.None)
        {
            for (int i = 0; i < skinGlasses.Count; i++)
            {
                if (skinGlasses[i] != null && skinGlasses[i].skinGlassesId == id)
                {
                    if (glass != null)
                    {
                        Destroy(glass.gameObject);
                    }

                    glass = Instantiate(skinGlasses[i], playerEquipment.tranGlass.position, playerEquipment.tranGlass.rotation);
                    glass.transform.SetParent(playerEquipment.tranGlass.transform);
                    return;
                }
            }
        }
        else
        {
            if (glass != null)
            {
                Destroy(glass.gameObject);
            }
        }
    }

    #endregion
}
