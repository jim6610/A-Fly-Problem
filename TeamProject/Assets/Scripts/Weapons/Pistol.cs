using UnityEngine;
using UnityEngine.UI;

/// Pistol ranged weapon
public class Pistol : ReloadableProjectileWeapon
{
    protected override string fireSoundTag => "GunShot";
    protected override string reloadSoundTag => "PistolReload";
    public override void Equipped() { }

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
            Fire();
        }

        // If player tries to fire an empty weapon, empty clip sound effect will play
        if (CanFire && ammoRemaining == 0)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            audioManager.Play("PistolClipEmpty");
        }

        // Update ammo display on HUD
        ammoDisplay.text = currentAmmoClip + " | " + ammoRemaining;
    }

    /// Weapon firing logic
    protected override void Fire()
    {
        hitEnemy = false;
        
        audioManager.Play(fireSoundTag);
        muzzleFlash.Play();

        currentAmmoClip--;

        SpawnBullet();

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out var hit, range))
        {
            if (hit.transform.CompareTag("Destructible"))
                // Damage destructible objects
                HandleDestructibleCollision(hit);

            if (hit.transform.CompareTag("Enemy"))
                // Damage enemy
                HandleEnemyCollision(hit);

            // Apply force to the object if it has a rigidbody attached
            HandleImpactForce(hit);
            // Instantiate Impact particle effect
            HandleImpactEffect(hit);

            if (!hitEnemy && !hit.transform.CompareTag("Web"))
                // Instantiate bullet decal effect
                HandleBulletDecal(hit);
        }
    }
}
