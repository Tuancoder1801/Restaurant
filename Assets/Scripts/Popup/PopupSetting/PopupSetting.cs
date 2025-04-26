using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PopupSetting : BasePopup
{
    public Slider sliderMusic;
    public Slider sliderSFX;

    protected override void Start()
    {   
        base.Start();

        sliderMusic.value = PlayerPrefs.GetFloat("music", 1f);
        sliderSFX.value = PlayerPrefs.GetFloat("sfx", 1f);

        sliderMusic.onValueChanged.AddListener((v) =>
        {
            AudioManager.Instance.SetMusicVolume(v);
        });

        sliderSFX.onValueChanged.AddListener((v) =>
        {
            AudioManager.Instance.SetSFXVolume(v);
        });
    }
}
