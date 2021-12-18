using System;
using UnityEngine;

public class Bat : MeleeWeapon
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    
    protected override string fireSoundTag => "Swat";
    protected override string hitSoundTag => "BatHit";
    
    protected void Update()
    {
        if (CanFire)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Fire();
        }
    }

    public override void Equipped()
    {
        ammoDisplay.text = UnlimitedAmmoText;
    }

    protected override void Fire()
    {
        audioManager.Play(fireSoundTag);
        
        animator.SetTrigger(Attack);

        (bool didHit, RaycastHit hit) = HandleSphereCast();
        
        if (didHit)
        {
            audioManager.Play(hitSoundTag);
            
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
        }
    }
    

}