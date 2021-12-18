using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Parameters")]
    [SerializeField] protected float damage = 2f;
    [SerializeField] protected float impactForce = 30f;
    [SerializeField] protected float fireRate = 2f;
    [SerializeField] protected float range = 5f;
    
    [Header("Animation")]
    [SerializeField] protected Animator animator;

    /// Tag used for the weapon fire sound effect in the AudioManager
    protected abstract string fireSoundTag { get; }

    protected AudioManager audioManager;
    protected Text ammoDisplay;
    protected Camera fpsCam;
    
    protected float nextTimeToFire;
    
    protected bool CanFire => Input.GetButton("Fire1") && Time.time >= nextTimeToFire;
    
    /// Logic to handle how the weapon behaves when fired
    protected abstract void Fire();
    
    /// Event called when the weapon becomes equipped
    public abstract void Equipped();
    
    protected virtual void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        ammoDisplay = GameObject.Find("Ammo").GetComponent<Text>();
        fpsCam = Camera.main;
    }
}
