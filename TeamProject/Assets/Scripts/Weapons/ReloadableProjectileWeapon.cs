using UnityEngine;
using System.Collections;

public abstract class ReloadableProjectileWeapon : Weapon
{
    [Header("Reload Parameters")]
    [SerializeField] protected float reloadTime = 3;
    [SerializeField] protected int clipSize = 6;
    [SerializeField] protected int ammoRemaining = 30;

    [Header("Effects")]
    [SerializeField] protected GameObject impactEffectParticle;
    [SerializeField] protected ParticleSystem muzzleFlash;
    [SerializeField] protected GameObject emitter;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected GameObject bulletHole;


    /// Tag used for the weapon reload sound effect in the AudioManager
    protected abstract string reloadSoundTag { get; }

    protected GameObject selectionManager;
    protected GameObject weaponManager;

    protected int currentAmmoClip;
    protected bool isReloading;
    protected bool hitEnemy;

    void Start()
    {
        selectionManager = GameObject.Find("SelectionManager");
        weaponManager = GameObject.Find("WeaponHolder");
        isReloading = false;
        currentAmmoClip = clipSize;
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    protected virtual void SpawnBullet()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, emitter.transform.position, emitter.transform.rotation);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.SetDirection(fpsCam.transform.forward);
    }

    protected virtual void HandleDestructibleCollision(RaycastHit hit)
    {
        Destructible target = hit.transform.GetComponent<Destructible>();

        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }

    protected virtual void HandleEnemyCollision(RaycastHit hit)
    {
        EnemyHealth target = hit.transform.GetComponentInChildren<EnemyHealth>();

        if (target != null)
        {
            target.TakeDamage(damage);
            hitEnemy = true;
        }
    }

    protected virtual void HandleImpactForce(RaycastHit hit)
    {
        Rigidbody rigidBody = hit.transform.GetComponent<Rigidbody>();

        if (rigidBody != null)
        {
            rigidBody.AddForce(-hit.normal * impactForce);
        }
    }

    protected virtual void HandleImpactEffect(RaycastHit hit)
    {
        GameObject impactEffect = Instantiate(impactEffectParticle, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impactEffect, 1);
    }

    protected virtual void HandleBulletDecal(RaycastHit hit)
    {
        GameObject bulletDecal = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(bulletDecal, 5f);
    }

    ///  Weapon reload logic
    protected IEnumerator Reload()
    {
        // Reload Started
        audioManager.Play(reloadSoundTag);
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
}
