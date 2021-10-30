using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {

    public float health = 50f;
    public GameObject destroyedVersion;	// Reference to the shattered version of the object

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Shatter();
        }
    }

    void Shatter()
    {
        Instantiate(destroyedVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
