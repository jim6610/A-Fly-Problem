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
    private float specialEnemeyChance = 0.3f;


    private Transform[] spawnTransforms;
    private int numberOfFlysSpawned;
    private int specialEnemiesCount;

    // Start is called before the first frame update
    void Start()
    {
        specialEnemiesCount = 0;
        numberOfFlysSpawned = 0;

        Transform[] tempTransforms = GetComponentsInChildren<Transform>();
        spawnTransforms = new Transform[tempTransforms.Length - 1];
        //remove this transform because tempTransforms also contains this transform
        int index = 0;
        foreach (Transform t in tempTransforms)
        {
            if (t != this.transform)
                spawnTransforms[index++] = t;
        }


        foreach (Transform t in spawnTransforms)
            StartCoroutine(SpawnRoutine(t));

        foreach (Transform t in spawnTransforms)
            StartCoroutine(SpecialEnemiesSpawnRoutine(t));
    }

    IEnumerator SpecialEnemiesSpawnRoutine(Transform location)
    {
        while (numberOfFlysSpawned < GameManager.startingNumberOfFlies)
        {
            float specialChance = Random.Range(0.0f, 1.0f);
            specialEnemiesCount = GameManager.scorpionCount + GameManager.spiderCount;
            if (specialEnemiesCount <= GameManager.maxNumberOfSpecials && specialChance <= specialEnemeyChance)
            {
                if (specialChance < specialEnemeyChance * 0.5f)
                {
                    GameObject enemy = Instantiate(scorpion, location.position, scorpion.transform.rotation, this.transform);
                    ScorpionNavigator nav = enemy.GetComponent<ScorpionNavigator>();
                    if (nav)
                    {
                        foreach (Transform t in spawnTransforms)
                            nav.AddSafePosition(t.position);
                    }
                    GameManager.IncrementScorpionCount();
                }
                else
                {
                    GameObject enemy = Instantiate(spider, location.position, spider.transform.rotation, this.transform);
                    SpiderNavigator nav = enemy.GetComponent<SpiderNavigator>();
                    if (nav)
                    {
                        foreach (Transform t in spawnTransforms)
                            nav.AddSafePosition(t.position);
                    }
                    GameManager.IncrementSpiderCount();
                }
            }

            yield return new WaitForSeconds(GameManager.specialSpawnRate);
        }
    }


    IEnumerator SpawnRoutine(Transform location)
    {
        while (numberOfFlysSpawned < GameManager.startingNumberOfFlies)
        {
            //print("fly count: " + GameManager.flyCount);
            //print("fly capacity: " + GameManager.flyCapacity);
            if (GameManager.flyCount < GameManager.flyCapacity)
            {
                GameObject enemy = Instantiate(fly, location.position, fly.transform.rotation, this.transform);
                FlyNavigator nav = enemy.GetComponent<FlyNavigator>();
                if (nav)
                {
                    foreach (Transform t in spawnTransforms)
                        nav.AddSafePosition(t.position);
                }
                GameManager.IncrementFlyCount();
                numberOfFlysSpawned++;
            }

            yield return new WaitForSeconds(GameManager.flySpawnRate);
        }
    }

    public void DecrimentSpecialEnemyCount()
    {
        specialEnemiesCount--;
    }
}
