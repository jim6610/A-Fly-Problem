using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenGlass : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void PlaySoundEffect()
    {
        audioManager.Play("GlassShards");
    }
}
