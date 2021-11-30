using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/**
 * If it looks like something isn't working, then check the "Collisions" property tab on the particle system
 * attached to the flamethrower prefab. Nobody will read this.
 */
public class FlameParticle : MonoBehaviour
{
    [SerializeField] private float damage = 0.25f;
    [SerializeField] private GameObject fireRemnantParticles;

    private ParticleSystem _particleSystem;
    private List<ParticleCollisionEvent> _collisionEvents;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth target = other.GetComponentInChildren<EnemyHealth>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }

            var effect = Instantiate(fireRemnantParticles, other.transform.position, Quaternion.Euler(-90, 0, 0), other.transform);
            Destroy(effect, 2.5f);
            
            return;
        }

        if (other.CompareTag("Destructible"))
        {
            Destructible target = other.GetComponent<Destructible>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }

            return;
        }

        var numCollisionEvents = _particleSystem.GetCollisionEvents(other, _collisionEvents);
        var i = 0;
        while (i < numCollisionEvents)
        {
            if (Random.value > 0.9)
            {
                Vector3 position = _collisionEvents[i].intersection;
                var effect = Instantiate(fireRemnantParticles, position, Quaternion.Euler(-90, 0, 0), null);
                Destroy(effect, 2.5f);
            }

            i++;
        }
    }
}