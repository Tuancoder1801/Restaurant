using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer audioMixer;
    public AudioSource audioMusic;
    public AudioSource audioSFX;
    public AudioSource audioButton;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("music", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("music", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("sfx", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("sfx", volume);
    }

    public void LoadVolume()
    {
        float music = PlayerPrefs.GetFloat("music", 1f);
        float sfx = PlayerPrefs.GetFloat("sfx", 1f);

        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    public void TurnOnButtonSound()
    {
        audioButton.Play();
    }
}
