using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {

    public float initialHealth;
    public float monetaryValue;
    public GameObject destroyedVersion;	// Reference to the shattered version of the object

    private float currentHealth;
    private bool firstThresholdReached;
    private bool secondThresholdReached;
    private bool thirdThresholdReached;

    private void Start()
    {
        currentHealth = initialHealth;
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
            LoseMoney();
        }
        // Less than 50% health will cause player to lose 25% of this object's monetary value. 
        if (currentHealth <= initialHealth * 0.50 && !secondThresholdReached)
        {
            secondThresholdReached = true;
            LoseMoney();
        }
        // Less than 75% health will cause player to lose 25% of this object's monetary value. 
        if (currentHealth <= initialHealth * 0.25 && !thirdThresholdReached)
        {
            thirdThresholdReached = true;
            LoseMoney();
        }


        if (currentHealth <= 0)
        {
            Shatter();
            LoseMoney();
        }
    }

    public void LoseMoney()
    {
        GameManager.ReduceContractValue(monetaryValue * 0.25f);
    }


    void Shatter()
    {
        Instantiate(destroyedVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
