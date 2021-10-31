using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_flySwatter : MonoBehaviour
{
    [SerializeField] private float damage = 2f;
    [SerializeField] private float impactForce = 30f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float range = 4f;
    [SerializeField] private GameObject impactEffectParticle;
    [SerializeField] private Animator animator;

    private Camera fpsCam;
    private float nextTimeToFire;
    private bool isAttacking;
    private float timer;

    public float attackDelay;

    private void Start()
    {
        fpsCam = Camera.main;
        isAttacking = false;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Swat();
        }

    }

    void Swat()
    {
        animator.SetTrigger("Attack"); //trigger our animation

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            // Damage destructible objects
            if (hit.transform.tag == "Destructible")
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
