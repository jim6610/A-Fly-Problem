using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject fly;
    [SerializeField]
    private GameObject scorpion;
    [SerializeField]
    private GameObject spider;

    [SerializeField]
    private int maxNumberOfEnemies;
    [SerializeField]
    private float specialEnemyChance = 0.3f;
    [SerializeField]
    private float spawnInterval = 3.0f;

    private Transform[] spawnTransforms;
    private int numberOfEnemiesSpawned;

    public void SetMaxNumberOfEnemies(int num)
    {
        maxNumberOfEnemies = num;
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform[] tempTransforms = GetComponentsInChildren<Transform>();
        spawnTransforms = new Transform[tempTransforms.Length - 1];
        //remove this transform because tempTransforms also contains this transform
        int index = 0;
        foreach(Transform t in tempTransforms)
        {
            if (t != this.transform)
                spawnTransforms[index++] = t;
        }
        numberOfEnemiesSpawned = 0;
        StartSpawnRoutine();
    }

    public void StartSpawnRoutine()
    {
        foreach (Transform t in spawnTransforms)
            StartCoroutine(SpawnRoutine(t));
    }

    IEnumerator SpawnRoutine(Transform location)
    {
        while(numberOfEnemiesSpawned<maxNumberOfEnemies)
        {
            float specialChance = Random.Range(0.0f, 1.0f);
            if (specialChance < specialEnemyChance)
            {
                if (specialChance < specialEnemyChance * 0.5f)
                    Instantiate(scorpion, location.position, scorpion.transform.rotation);
                else
                    Instantiate(spider, location.position, spider.transform.rotation);
            }
            else
                Instantiate(fly, location.position, fly.transform.rotation);
            numberOfEnemiesSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
