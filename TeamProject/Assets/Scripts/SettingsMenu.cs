using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    private void Awake()
    {
        Debug.Log("Volume " + AudioListener.volume);
        volumeSlider.value = PlayerPrefs.GetFloat("master_volume", AudioListener.volume);
    }

    public void SetVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("master_volume", newVolume);
        AudioListener.volume = newVolume;
        Debug.Log("Volume " + AudioListener.volume);
    }
}
