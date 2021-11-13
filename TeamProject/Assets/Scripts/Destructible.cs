using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {

    [SerializeField] private float initialHealth;
    [SerializeField] private float monetaryValue;
    [SerializeField] private float breakForce;
    public GameObject destroyedVersion;	// Reference to the shattered version of the object

    private float currentHealth;
    private float currentValue;
    private bool firstThresholdReached;
    private bool secondThresholdReached;
    private bool thirdThresholdReached;

    private void Start()
    {
        currentHealth = initialHealth;
        currentValue = monetaryValue;
        firstThresholdReached = false;
        secondThresholdReached = false;
        thirdThresholdReached = false;
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Less than 25% health will cause player to lose 25% of this object's monetary value. 
        if (currentHealth <= initialHealth * 0.75 && !firstThresholdReached)
        {
            firstThresholdReached = true;
            LoseQuarterValue();
        }
        // Less than 50% health will cause player to lose 25% of this object's monetary value. 
        if (currentHealth <= initialHealth * 0.50 && !secondThresholdReached)
        {
            secondThresholdReached = true;
            LoseQuarterValue();
        }
        // Less than 75% health will cause player to lose 25% of this object's monetary value. 
        if (currentHealth <= initialHealth * 0.25 && !thirdThresholdReached)
        {
            thirdThresholdReached = true;
            LoseQuarterValue();
        }


        if (currentHealth <= 0)
        {
            Shatter();
            LoseQuarterValue();
        }
    }

    public void LoseQuarterValue()
    {
        GameManager.ReduceContractValue(monetaryValue * 0.25f);
        currentValue -= monetaryValue * 0.25f;
    }

    public void LoseRemainingValue()
    {
        GameManager.ReduceContractValue(currentValue);
    }

    void Shatter()
    {
        Instantiate(destroyedVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        Vector3 impactForce = other.relativeVelocity;

        if (impactForce.x >= breakForce || impactForce.y >= breakForce || impactForce.z >= breakForce)
        {
            if (other.collider.CompareTag("Enemy"))
            {
                other.collider.GetComponent<EnemyHealth>().TakeDamage(initialHealth);
                this.TakeDamage(initialHealth);
            }
            else if (other.collider.CompareTag("Destructible"))
            {
                other.collider.GetComponent<Destructible>().TakeDamage(initialHealth);
                this.TakeDamage(initialHealth);
            }
            else
            {
                this.TakeDamage(initialHealth);
            }
        }
    }

}
