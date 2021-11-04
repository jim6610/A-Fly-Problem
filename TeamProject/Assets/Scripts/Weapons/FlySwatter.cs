using UnityEngine;

/// FlySwatter melee weapon
/// TODO Code not DRY, shares almost all game variables/logic with Pistol, can probably use inheritance here
public class FlySwatter : MonoBehaviour
{
    [Header("Weapon Parameters")]
    [SerializeField] private float damage = 2f;
    [SerializeField] private float impactForce = 30f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float range = 4f;
    [Header("Effects")]
    [SerializeField] private GameObject impactEffectParticle;
    [Header("Animation")]
    [SerializeField] private Animator animator;
    private AudioManager audioManager;

    private Camera fpsCam;
    private float nextTimeToFire;
    
    private static readonly int Attack = Animator.StringToHash("Attack");
    
    private bool CanFire => Input.GetButton("Fire1") && Time.time >= nextTimeToFire;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        
        fpsCam = Camera.main;
    }
    
    void Update()
    {
        if (CanFire)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Swat();
        }
    }

    /// Weapon firing logic
    void Swat()
    {
        audioManager.Play("Swat");
        
        animator.SetTrigger(Attack); //trigger our animation

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out var hit, range))
        {
            audioManager.Play("Smack");
            
            // Damage destructible objects
            if (hit.transform.CompareTag("Destructible"))
            {
                Destructible target = hit.transform.GetComponent<Destructible>();

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
