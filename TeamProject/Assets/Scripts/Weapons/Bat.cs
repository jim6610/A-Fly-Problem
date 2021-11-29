using UnityEngine;
using UnityEngine.UI;

public class Bat : MonoBehaviour
{
   [Header("Weapon Parameters")]
    [SerializeField] private float damage = 2f;
    [SerializeField] private float impactForce = 30f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float range = 5f;

    [SerializeField] private GameObject bulletHole;

    [Header("Effects")]
    [SerializeField] private GameObject impactEffectParticle;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private AudioManager audioManager;
    private Text ammoDisplay;

    private Camera fpsCam;
    private float nextTimeToFire;
    
    private static readonly int Attack = Animator.StringToHash("Attack");
    
    private bool CanFire => Input.GetButton("Fire1") && Time.time >= nextTimeToFire;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        ammoDisplay = GameObject.Find("Ammo").GetComponent<Text>();

        fpsCam = Camera.main;
    }
    
    void Update()
    {
        if (CanFire)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Swat();
        }

        // Update ammo display on HUD
        ammoDisplay.text = "-- | --";
    }

    /// Weapon firing logic
    void Swat()
    {
        audioManager.Play("Swat");
        
        animator.SetTrigger(Attack); //trigger our animation
        
        Debug.DrawLine(fpsCam.transform.position, fpsCam.transform.position + (fpsCam.transform.forward.normalized * range), Color.green, 2);
        if (Physics.SphereCast(fpsCam.transform.position, 0.1f, fpsCam.transform.forward, out var hit, range))
        {
            audioManager.Play("BatHit");
            
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
            GameObject bulletDecal = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactEffect, 1);
            Destroy(bulletDecal, 5f);
        }
    }
}
