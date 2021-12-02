using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> lightSources;
    
    private AudioSource _toggleSound;
    private Light _playerLight;

    private bool _lightOn = true;

    public bool LightOn => _lightOn;

    private void Awake()
    {
        _playerLight = FindObjectOfType<PlayerMovement>().GetComponentInChildren<Light>();
        _toggleSound = GetComponent<AudioSource>();
        
        // Turn on every light on awake to avoid potential problems
        foreach (var lightSource in lightSources)
        {
            lightSource.SetActive(true);
        }
    }

    public void ToggleLight()
    {
        _toggleSound.Play();

        _lightOn = !_lightOn;
        _playerLight.enabled = !_lightOn;
        
        foreach (var lightSource in lightSources)
        {
            lightSource.SetActive(!lightSource.activeSelf);
        }
    }
}
