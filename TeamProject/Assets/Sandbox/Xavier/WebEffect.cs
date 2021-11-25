using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebEffect : MonoBehaviour
{
    public float timeUntilDestruction = 12f; //web will be detroyed automatically within x number of seconds

    void Start()
    {
        StartCoroutine(selfDestroy());
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

    private IEnumerator selfDestroy()
    {
       
        while (timeUntilDestruction >= 0f)
        {
            yield return new WaitForSeconds(1);
            timeUntilDestruction -= 1f;
        }

        Destroy(this.gameObject);
    }
}
