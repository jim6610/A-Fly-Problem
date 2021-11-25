using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebEffect : MonoBehaviour
{

    public float debuffDuration = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(selfDestroy());
    }

    void OnTriggerEnter(Collider collider)
    {
        PlayerMovement movement = collider.GetComponentInParent<PlayerMovement>();
        if(movement)
        {
            /*Debug.Log("web stepped on");*/
            movement.StartMovementSlowdown(debuffDuration);
            Destroy(this.gameObject);
        }                
    }

    private IEnumerator selfDestroy()
    {
        float timeUntilDestruction = 10f; //web will be detroyed automatically 10 seconds after spawning

        while (timeUntilDestruction >= 0f)
        {
            /*Debug.Log(timeUntilDestruction);*/
            yield return new WaitForSeconds(1);
            timeUntilDestruction -= 1f;
        }

        Destroy(this.gameObject);
    }
}
