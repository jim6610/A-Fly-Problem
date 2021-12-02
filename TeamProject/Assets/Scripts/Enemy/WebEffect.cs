using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebEffect : MonoBehaviour
{
    public float timeUntilDestruction = 12f; //web will be detroyed automatically within x number of seconds

    void Start()
    {
        Destroy(transform.parent.gameObject, timeUntilDestruction);
    }

    void OnTriggerEnter(Collider collider)
    {
        PlayerMovement movement = collider.GetComponentInParent<PlayerMovement>();
        if(movement)
        {
            movement.StartMovementSlowdown();
            Destroy(this.gameObject);
        }                
    }
}
