using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private float breakForce = 8.0f;

    [SerializeField] private GameObject brokenModel;

    void Break()
    {
        Debug.Log("BROKEN");
        
        // TODO Instantiate your broken model here and then delete this gameobject
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Solid"))
        {
            Vector3 impactForce = other.relativeVelocity;
            
            Debug.Log("Force = " + other.relativeVelocity);
            
            if (impactForce.x >= breakForce || impactForce.y >= breakForce || impactForce.z >= breakForce)
                Break();
        }
    }

}