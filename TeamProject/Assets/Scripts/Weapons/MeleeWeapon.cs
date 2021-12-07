using System;
using UnityEngine;

public abstract class MeleeWeapon : Weapon
{
    [Header("Weapon Parameters")]
    [SerializeField] protected float sphereCastRadius = 0.1f;
    
    protected abstract string hitSoundTag { get; }
    
    protected const string UnlimitedAmmoText = "-- | --";

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

    protected Tuple<bool, RaycastHit> HandleSphereCast(bool debug = false)
    {
        Transform cameraTransform = fpsCam.transform;

        if (debug)
        {
            Debug.DrawLine(
                fpsCam.transform.position, 
                cameraTransform.position + (cameraTransform.forward.normalized * range), 
                Color.green, 
                2);
        }
            
        
        bool didHit = Physics.SphereCast(
            cameraTransform.position, 
            sphereCastRadius, 
            cameraTransform.forward, 
            out var hit, 
            range);

        return Tuple.Create(didHit, hit);
    }
}
