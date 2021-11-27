using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Flamethrower : MonoBehaviour
{
    [Header("Weapon Parameters")]
    [SerializeField] private float damage = 4f;
    [SerializeField] private float impactForce = 75f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float reloadTime = 6;
    [SerializeField] private int totalAmmo = 3000;

    [Header("Effects")]
    [SerializeField] private ParticleSystem starterParticles;
    [SerializeField] private ParticleSystem attackParticles;

    private AudioManager audioManager;
    private GameObject weaponManager;
    private GameObject selectionManager;
    private Text ammoDisplay;

    private Camera fpsCam;
    private float nextTimeToFire;
    private int currentAmmoClip;
    private bool isReloading = false;

    private bool IsFiring => Input.GetButton("Fire1");
    private bool firstShot = true;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        weaponManager = GameObject.Find("WeaponHolder");
        selectionManager = GameObject.Find("SelectionManager");
        ammoDisplay = GameObject.Find("Ammo").GetComponent<Text>();
        fpsCam = Camera.main;
        currentAmmoClip = totalAmmo;
    }

    void Update()
    {
        // If able to fire and ammo clip is not empty, fire weapon
        if (IsFiring && currentAmmoClip != 0)
        {
            Shoot();
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


    /// Weapon firing logic
    void Shoot()
    {
        if (firstShot)
        {
            firstShot = false;
            audioManager.Play("Flamethrower");
            attackParticles.Play();
        }

        currentAmmoClip--;

        
    }

    void StopShooting()
    {
        firstShot = true;
        audioManager.Stop("Flamethrower");
        attackParticles.Stop();
    }
}
