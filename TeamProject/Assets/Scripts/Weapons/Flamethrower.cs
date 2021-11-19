using UnityEngine;
using System.Collections;


/// Rifle ranged weapon
/// TODO Code not DRY, shares almost all game variables/logic with FlySwatter, can probably use inheritance here
public class Flamethrower : MonoBehaviour
{
    [Header("Weapon Parameters")]
    [SerializeField] private float damage = 4f;
    [SerializeField] private float impactForce = 75f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float reloadTime = 6;
    [SerializeField] private int ammoRemaining = 3000;

    [Header("Effects")]
    [SerializeField] private ParticleSystem starterParticles;
    [SerializeField] private ParticleSystem attackParticles;
    private AudioManager audioManager;
    private GameObject weaponManager;
    private GameObject selectionManager;

    private Camera fpsCam;
    private float nextTimeToFire;
    private int currentAmmoClip;
    private bool isReloading = false;
    private CapsuleCollider areaOfEffect;

    private bool IsFiring => Input.GetButton("Fire1");
    private bool firstShot = true;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        weaponManager = GameObject.Find("WeaponHolder");
        selectionManager = GameObject.Find("SelectionManager");
        fpsCam = Camera.main;
        currentAmmoClip = ammoRemaining;
        areaOfEffect = gameObject.GetComponent<CapsuleCollider>();
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
        if(currentAmmoClip == 0)
        {
            starterParticles.Stop();
        }
    }


    /// Weapon firing logic
    void Shoot()
    {
        if (firstShot)
        {
            firstShot = false;
            audioManager.Play("Flamethrower");
            attackParticles.Play();
            areaOfEffect.enabled = true;
        }

        currentAmmoClip--;

        
    }

    void StopShooting()
    {
        firstShot = true;
        audioManager.Stop("Flamethrower");
        attackParticles.Stop();
        areaOfEffect.enabled = false;
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.collider.name);
    }
}
