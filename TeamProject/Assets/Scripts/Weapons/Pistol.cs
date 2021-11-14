using UnityEngine;
using System.Collections;


/// Pistol ranged weapon
/// TODO Code not DRY, shares almost all game variables/logic with FlySwatter, can probably use inheritance here
public class Pistol : MonoBehaviour
{
    [Header("Weapon Parameters")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float impactForce = 75f;
    [SerializeField] private float fireRate = 5f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float reloadTime = 3;
    [SerializeField] private int clipSize = 6;
    [SerializeField] private int ammoRemaining = 30;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffectParticle;
    [SerializeField] private GameObject bulletPrefab;
    private AudioManager audioManager;
    private GameObject weaponManager;
    private GameObject selectionManager;
    public Animator animator;

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
            audioManager.Play("PistolClipEmpty");
        }
    }
    
    ///  Weapon reload logic
    IEnumerator Reload()
    {
        // Reload Started
        audioManager.Play("PistolReload");
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
        currentAmmoClip = clipSize;
    }

    /// Weapon firing logic
    void Shoot()
    {
        audioManager.Play("GunShot");
        muzzleFlash.Play();

        currentAmmoClip--;
        ammoRemaining--;

        GameObject bulletObj = Instantiate(bulletPrefab);
        bulletObj.transform.position = transform.position;

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
                }
            }
            // Damage enemy
            else if (hit.transform.CompareTag("Enemy"))
            {
                EnemyHealth target = hit.transform.GetComponentInChildren<EnemyHealth>();

                if (target != null)
                {
                    target.TakeDamage(damage);
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
        }
    }
}
