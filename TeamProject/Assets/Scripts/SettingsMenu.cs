using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    
    [SerializeField] private AudioMixer musicAudioMixer;
    [SerializeField] private AudioMixer soundAudioMixer;

    Func<float, float> asDecibels = volume => Mathf.Log10(volume) * 20;
    private void Start()
    {
        // You can't update audiomixer volume in awake apparently it's a bug
        UpdateState();
    }

    public void SetMusicVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("music_volume", newVolume);
        musicAudioMixer.SetFloat("volume", asDecibels(newVolume));
    }
    
    public void SetSoundVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("sound_volume", newVolume);
        soundAudioMixer.SetFloat("volume", asDecibels(newVolume));
    }

    public void SetFullScreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        Screen.fullScreen = isFullscreen;
    }

    public void UpdateState()
    {
        const int defaultVolume = 1;
        
        soundSlider.value = PlayerPrefs.GetFloat("sound_volume", defaultVolume);
        musicSlider.value = PlayerPrefs.GetFloat("music_volume", defaultVolume);

        musicAudioMixer.SetFloat("volume", asDecibels(musicSlider.value));
        soundAudioMixer.SetFloat("volume", asDecibels(soundSlider.value));
    }
}
