using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private Dropdown resolutionDropdown;

    [SerializeField] private Toggle fullscreenToggle;

    private Resolution[] _resolutions;

    Func<float, float> asDecibels = volume => Mathf.Log10(volume) * 20;
    private void Start()
    {
        // You can't update audiomixer volume in awake apparently it's a bug
        VolumeStateHandler();
        ResolutionHandler();
        // resolutionDropdown.AddOptions(
        //     _resolutions.Select(resolution => $"{resolution.width} x {resolution.height}").ToList());
    }

    private void ResolutionHandler()
    {
        _resolutions = GetResolutions().ToArray();
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        var currentResolutionIndex = 0;
        
        for (var i = 0; i < _resolutions.Length; i++)
        {
            options.Add($"{_resolutions[i].width} x {_resolutions[i].height}");

            // If the resolution matches what we currently have set then add it.
            if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
            {
                fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;

                Screen.SetResolution(_resolutions[i].width, _resolutions[i].height, PlayerPrefs.GetInt("fullscreen", 1) == 1);
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
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

    public void VolumeStateHandler()
    {
        const int defaultVolume = 1;
        
        soundSlider.value = PlayerPrefs.GetFloat("sound_volume", defaultVolume);
        musicSlider.value = PlayerPrefs.GetFloat("music_volume", defaultVolume);

        musicAudioMixer.SetFloat("volume", asDecibels(musicSlider.value));
        soundAudioMixer.SetFloat("volume", asDecibels(soundSlider.value));
    }
    
    // http://answers.unity.com/answers/1712772/view.html
    public static List<Resolution> GetResolutions() {
        //Filters out all resolutions with low refresh rate:
        Resolution[] resolutions = Screen.resolutions;
        HashSet<Tuple<int, int>> uniqResolutions = new HashSet<Tuple<int, int>>();
        Dictionary<Tuple<int, int>, int> maxRefreshRates = new Dictionary<Tuple<int, int>, int>();
        
        for (int i = 0; i < resolutions.GetLength(0); i++)
        {
            if (resolutions[i].width < 1024) continue;
            
            //Add resolutions (if they are not already contained)
            Tuple<int, int> resolution = new Tuple<int, int>(resolutions[i].width, resolutions[i].height);
            uniqResolutions.Add(resolution);
            
            //Get highest framerate:
            if (!maxRefreshRates.ContainsKey(resolution)) {
                maxRefreshRates.Add(resolution, resolutions[i].refreshRate);
            } else {
                maxRefreshRates[resolution] = resolutions[i].refreshRate;
            }
        }
        
        //Build resolution list:
        List<Resolution> uniqResolutionsList = new List<Resolution>(uniqResolutions.Count);
        
        foreach (Tuple<int, int> resolution in uniqResolutions) {
            Resolution newResolution = new Resolution();
            newResolution.width = resolution.Item1;
            newResolution.height = resolution.Item2;
            
            if(maxRefreshRates.TryGetValue(resolution, out int refreshRate)) {
                newResolution.refreshRate = refreshRate;
            }
            
            uniqResolutionsList.Add(newResolution);
        }
        return uniqResolutionsList;
    }
}
