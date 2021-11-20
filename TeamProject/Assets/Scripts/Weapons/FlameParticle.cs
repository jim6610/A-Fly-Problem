using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * If it looks like something isn't working, then check the "Collisions" property tab on the particle system
 * attached to the flamethrower prefab. Nobody will read this.
 */
public class FlameParticle : MonoBehaviour
{
    [SerializeField] private float damage = 0.25f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth target = other.GetComponentInChildren<EnemyHealth>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
        
        if (other.CompareTag("Destructible"))
        {
            Debug.Log("Lemao");
            
            Destructible target = other.GetComponent<Destructible>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }
}
