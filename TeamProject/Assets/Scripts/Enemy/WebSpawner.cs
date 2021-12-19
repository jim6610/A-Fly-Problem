using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebSpawner : MonoBehaviour
{
    public GameObject web;
    public Transform spawnPos;
    public float spawnInterval = 5.0f;

    void Start()
    {
        InvokeRepeating("SpawnWeb", spawnInterval, spawnInterval);
    }

    void SpawnWeb()
    {
        Instantiate(web, spawnPos.position, Quaternion.Euler(-90, 0, 0));
    }

}
