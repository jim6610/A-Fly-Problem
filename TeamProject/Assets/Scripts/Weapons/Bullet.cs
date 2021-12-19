using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;
    public float expirationTime;


    private void Start()
    {
        Destroy(gameObject, expirationTime);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Set a default direction
        rb.velocity = Vector3.forward * speed;
    }

    public void SetDirection(Vector3 direction)
    {
        rb.velocity = direction * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
