using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;

    void Start()
    {
        health *= GameManager.enemyHealthModifier; // Depending on difficulty, health will be adjusted
    }

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
        FlyRelativeMovmement frm = GetComponent<FlyRelativeMovmement>();
        if (frm)
            frm.SetFlyMode(FlyMode.DEATH);

        SpiderNavigator spn = GetComponent<SpiderNavigator>();
        if (spn)
            spn.SetDead();

        ScorpionNavigator scn = GetComponent<ScorpionNavigator>();
        if (scn)
            scn.SetDead();
    }
}
