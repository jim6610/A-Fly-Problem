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
    [SerializeField] private float reloadTime = 5;
    [SerializeField] private int maxAmmo = 20;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffectParticle;
    [SerializeField] private GameObject bulletPrefab;
    private AudioManager audioManager;
    public Animator animator;

    private Camera fpsCam;
    private float nextTimeToFire;
    private int currentAmmo;
    private bool isReloading = false;

    private bool CanFire => Input.GetButton("Fire1") && Time.time >= nextTimeToFire;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        fpsCam = Camera.main;
        currentAmmo = maxAmmo;
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    void Update()
    {
        if (isReloading)
        {
            return;
        }


        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (CanFire)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Shoot();
        }
    }

    ///  Weapon reload logic
    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime - 0.25f);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(0.25f);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    /// Weapon firing logic
    void Shoot()
    {
        audioManager.Play("GunShot");
        muzzleFlash.Play();

        currentAmmo--;

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
