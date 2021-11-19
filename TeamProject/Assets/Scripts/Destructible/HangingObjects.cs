using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingObjects : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        this.GetComponent<Rigidbody>().useGravity = true;
    }
}
