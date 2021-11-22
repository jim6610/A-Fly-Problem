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
    private float spawnInterval = 3.0f;
    [SerializeField]
    private float specialEnemeyChance = 0.1f;

    private Transform[] spawnTransforms;
    private int numberOfEnemiesSpawned;
    private int specialEnemiesCount;
    private int maxNumberOfFlies;
    private int maxSpecialEnemyAmount;

    // Start is called before the first frame update
    void Start()
    {
        maxNumberOfFlies = GameManager.startingNumberOfFlies;
        maxSpecialEnemyAmount = GameManager.maxNumberOfSpecials;
        specialEnemiesCount = 0;

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


        foreach (Transform t in spawnTransforms)
            StartCoroutine(SpawnRoutine(t));
    }


    IEnumerator SpawnRoutine(Transform location)
    { 
        while(numberOfEnemiesSpawned<maxNumberOfFlies)
        {

            float specialChance = Random.Range(0.0f, 1.0f);
            if (specialEnemiesCount <= maxSpecialEnemyAmount && specialChance <= specialEnemeyChance)
            {
                if (specialChance < specialEnemeyChance * 0.5f)
                {
                    Instantiate(scorpion, location.position, scorpion.transform.rotation);
                    GameManager.IncrementScorpionCount();
                    specialEnemiesCount++;
                }
                else
                {
                    Instantiate(spider, location.position, spider.transform.rotation);
                    GameManager.IncrementSpiderCount();
                    specialEnemiesCount++;
                }
            }
            else
            {
                Instantiate(fly, location.position, fly.transform.rotation);
                GameManager.IncrementFlyCount();
                numberOfEnemiesSpawned++;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void DecrimentSpecialEnemyCount()
    {
        specialEnemiesCount--;
    }
}
