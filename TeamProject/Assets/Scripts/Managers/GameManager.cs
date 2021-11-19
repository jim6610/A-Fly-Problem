using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // *** Global game variables ***
    // Money related variables
    public static float currentContractValue; // How much money player is getting upon finishing the level. Can be reduced if destructible objects are damaged/destroyed
    public static float totalDamageCosts; // Amount of money lost due to destroyed objects
    public static float penaltyModifier; // 1.0 for normal. 2.0 for hard

    // Starting Number of flies for the level
    public static int startingNumberOfFlies;

    // Number of enemies remaining on level
    public static int flyCount;
    public static int spiderCount;
    public static int scorpionCount;

    // Kill count
    public static int flyKillCount;
    public static int spiderKillCount;
    public static int scorpionKillCount;

    // Enemy Stats (dependent on difficulty)
    public static float enemyHealthModifier = 1.0f; // 1.0 for normal, 2.0 for hard
    public static float enemySpeedModifier;
    public static float spawnChanceModifier;

    // *******************************


    // Level variables
    [SerializeField] private float contractValue;
    [SerializeField] private int setStartingNumberOfFlies;

    // HUD text elements
    [SerializeField] private Text moneyDisplay;
    [SerializeField] private Text flyCounterDisplay;


    void Start()
    {
        currentContractValue = contractValue;
        startingNumberOfFlies = setStartingNumberOfFlies;

        // TODO: get difficulty selected
        Debug.Log(PlayerPrefs.GetString("difficulty"));
    }

    // Update is called once per frame
    void Update()
    {
        // Update HUD elements
        moneyDisplay.text = String.Format("{0:.00}", currentContractValue);
        flyCounterDisplay.text = flyCount.ToString();
    }



    // Contract modifier functions
    public static void IncreaseContractValue(float bonus)
    {
        currentContractValue += bonus;
    }
    public static void ReduceContractValue(float penalty)
    {
        currentContractValue -= penalty;
        totalDamageCosts += penalty;
    }

    // Enemy counter modifier functions
    public static void IncrementFlyCount()
    {
        flyCount++;
    }
    public static void IncrementSpiderCount()
    {
        spiderCount++;
    }
    public static void IncrementScorpionCount()
    {
        scorpionCount++;
    }
    public static void DecrementFlyCount()
    {
        flyCount--;
    }
    public static void DecrementSpiderCount()
    {
        spiderCount--;
    }
    public static void DecrementScorpionCount()
    {
        scorpionCount--;
    }

    // Enemy kill count functions
    public static void IncrementFlyKillCount()
    {
        flyKillCount++;
    }
    public static void IncrementSpiderKillCount()
    {
        spiderKillCount++;
    }
    public static void IncrementScorpionKillCount()
    {
        scorpionKillCount++;
    }
}
