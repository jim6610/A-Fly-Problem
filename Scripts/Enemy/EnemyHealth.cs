using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    public void Die()
    {
        Destroy(transform.parent.gameObject);
    }
}
