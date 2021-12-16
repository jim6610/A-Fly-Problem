using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Flamethrower : Weapon
{
    [Header("Weapon Parameters")]
    [SerializeField] private int totalAmmo = 3000;

    [Header("Effects")]
    [SerializeField] private ParticleSystem starterParticles;
    [SerializeField] private ParticleSystem attackParticles;
    
    private int currentAmmoClip;

    private bool IsFiring => Input.GetButton("Fire1");
    private bool firstShot = true;
    
    protected override string fireSoundTag => "Flamethrower";

    public override void Equipped() {}

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        ammoDisplay = GameObject.Find("Ammo").GetComponent<Text>();
        fpsCam = Camera.main;
        currentAmmoClip = totalAmmo;
    }

    void Update()
    {
        // If able to fire and ammo clip is not empty, fire weapon
        if (IsFiring && currentAmmoClip != 0)
        {
            Fire();
        }
        else
        {
            StopShooting();
        }
        
        if (IsFiring && currentAmmoClip == 0)
        {
            audioManager.Play("RifleClipEmpty");
        }
        
        if (currentAmmoClip == 0)
        {
            starterParticles.Stop();
        }

        // Update ammo display on HUD
        ammoDisplay.text = currentAmmoClip + " | 0";
    }
    
    protected override void Fire()
    {
        if (firstShot)
        {
            firstShot = false;
            audioManager.Play(fireSoundTag);
            attackParticles.Play();
        }

        currentAmmoClip--;
    }

    void StopShooting()
    {
        firstShot = true;
        audioManager.Stop(fireSoundTag);
        attackParticles.Stop();
    }
}
