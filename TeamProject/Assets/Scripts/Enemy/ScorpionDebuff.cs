using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionDebuff : MonoBehaviour
{

    [SerializeField]
    private float duration = 3.0f;

    private GameObject player;

    void Start()
    {
        player = (GameObject.FindObjectOfType<PlayerMovement>()).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            PlayerMovement movement = other.gameObject.GetComponent<PlayerMovement>();
            if (movement)
                StartCoroutine(movement.ApplyMovementReverse(duration));
        }
    }
}
