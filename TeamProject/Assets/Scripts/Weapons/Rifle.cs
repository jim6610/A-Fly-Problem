using UnityEngine;
using System.Collections;


/// Pistol ranged weapon
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
    [SerializeField] private GameObject bulletPrefab;
    private AudioManager audioManager;
    public Animator animator;

    private Camera fpsCam;
    private float nextTimeToFire;
    private int currentAmmoClip;
    private bool isReloading = false;

    private bool CanFire => Input.GetButton("Fire1") && Time.time >= nextTimeToFire;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
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
    }

    ///  Weapon reload logic
    IEnumerator Reload()
    {
        audioManager.Play("RifleReload");
        isReloading = true;
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime - 0.25f);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(0.25f);
        currentAmmoClip = clipSize;
        isReloading = false;
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
