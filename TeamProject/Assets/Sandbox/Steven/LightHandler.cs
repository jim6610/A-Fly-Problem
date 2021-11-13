using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> lightSources;
    private AudioSource _toggleSound;

    private void Awake()
    {
        _toggleSound = GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleLight()
    {
        _toggleSound.Play();
        foreach (var lightSource in lightSources)
        {
            lightSource.SetActive(!lightSource.activeSelf);
        }
    }
}
