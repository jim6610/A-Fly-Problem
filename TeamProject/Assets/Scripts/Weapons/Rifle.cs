using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// Rifle ranged weapon
/// TODO Code not DRY, shares almost all game variables/logic with FlySwatter, can probably use inheritance here
public class Rifle : MonoBehaviour
{
    [Header("Weapon Parameters")]
    [SerializeField] private float damage = 4f;
    [SerializeField] private float impactForce = 75f;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float reloadTime = 6;
    [SerializeField] private int clipSize = 20;
    [SerializeField] private int ammoRemaining = 60;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffectParticle;
    [SerializeField] private GameObject emitter;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletHole;

    [Header("Animation")]
    public Animator animator;

    private AudioManager audioManager;
    private GameObject weaponManager;
    private GameObject selectionManager;
    private Text ammoDisplay;

    private Camera fpsCam;
    private float nextTimeToFire;
    private int currentAmmoClip;
    private bool isReloading = false;

    private bool CanFire => Input.GetButton("Fire1") && Time.time >= nextTimeToFire;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        weaponManager = GameObject.Find("WeaponHolder");
        selectionManager = GameObject.Find("SelectionManager");
        ammoDisplay = GameObject.Find("Ammo").GetComponent<Text>();
        fpsCam = Camera.main;
        currentAmmoClip = clipSize;
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    void Update()
    {
        // If weapon is still in process of being reloaded, do nothing
        if (isReloading)
        {
            return;
        }

        // If weapon clip is empty and there is still ammo remaining, reload
        if (currentAmmoClip == 0 && ammoRemaining != 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // If able to fire and ammo clip is not empty, fire weapon
        if (CanFire && currentAmmoClip != 0)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Shoot();
        }

        // If player tries to fire an empty weapon, empty clip sound effect will play
        if (CanFire && ammoRemaining == 0)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            audioManager.Play("RifleClipEmpty");
        }

        // Update ammo display on HUD
        ammoDisplay.text = currentAmmoClip + " | " + ammoRemaining;
    }

    ///  Weapon reload logic
    IEnumerator Reload()
    {
        // Reload Started
        audioManager.Play("RifleReload");
        isReloading = true;
        animator.SetBool("Reloading", true);
        weaponManager.GetComponent<WeaponSwitching>().ToggleWeaponSwitching();
        selectionManager.GetComponent<SelectionManager>().ToggleIsReloading();
        yield return new WaitForSeconds(reloadTime - 0.25f);

        // Reload completed
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(0.25f); // this wait is to prevent weapon from firing before the animation is complete
        isReloading = false;
        weaponManager.GetComponent<WeaponSwitching>().ToggleWeaponSwitching();
        selectionManager.GetComponent<SelectionManager>().ToggleIsReloading();
        ammoRemaining -= clipSize;
        currentAmmoClip = clipSize;
    }

    /// Weapon firing logic
    void Shoot()
    {
        var hitObject = false;
        
        audioManager.Play("RifleShot");
        muzzleFlash.Play();

        currentAmmoClip--;

        // Bullet logic
        GameObject bulletObj = Instantiate(bulletPrefab, emitter.transform.position, emitter.transform.rotation);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.SetDirection(fpsCam.transform.forward);

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out var hit, range))
        {
            // Damage destructible objects
            if (hit.transform.CompareTag("Destructible"))
            {
                Destructible target = hit.transform.GetComponent<Destructible>();

                if (target != null)
                {
                    target.TakeDamage(damage);
                    hitObject = true;
                }
            }
            // Damage enemy
            else if (hit.transform.CompareTag("Enemy"))
            {
                EnemyHealth target = hit.transform.GetComponentInChildren<EnemyHealth>();

                if (target != null)
                {
                    target.TakeDamage(damage);
                    hitObject = true;
                }
            }

            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(-hit.normal * impactForce);
            }

            // Impact effect
            GameObject impactEffect = Instantiate(impactEffectParticle, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactEffect, 1);

            if (!hitObject && !hit.transform.CompareTag("Web"))
            {
                GameObject bulletDecal = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal)); 
                Destroy(bulletDecal, 5f); 
            }
        }
    }
}
