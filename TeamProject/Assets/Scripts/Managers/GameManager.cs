using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("TEMP")]
    [SerializeField] bool hardMode; // temporary. for testing purposes. TO DELETE

    [Header("Normal Mode Variables")]
    [SerializeField] private float contractValueNormal;
    [SerializeField] private float levelTimerNormal;
    [SerializeField] private int totalNumberOfFliesNormal;
    [SerializeField] private int maxNumberOfSpecialsNormal;
    [SerializeField] private float enemyHealthModifierNormal = 1.0f;
    [SerializeField] private float enemySpeedModifierNormal = 1.0f;
    [SerializeField] private float spawnChanceModifierNormal = 1.0f;
    [SerializeField] private float moneyPenaltyModifierNormal = 1.0f;

    [Header("Hard Mode Variables")]
    [SerializeField] private float contractValueHard;
    [SerializeField] private float levelTimerHard;
    [SerializeField] private int totalNumberOfFliesHard;
    [SerializeField] private int maxNumberOfSpecialsHard;
    [SerializeField] private float enemyHealthModifierHard;
    [SerializeField] private float enemySpeedModifierHard;
    [SerializeField] private float spawnChanceModifierHard;
    [SerializeField] private float moneyPenaltyModifierHard;

    [Header("HUD Text Objects")]
    [SerializeField] private Text moneyDisplay;
    [SerializeField] private Text flyCounterDisplay;


    // *** Global game variables ***

    // Difficulty modifiers
    public static float currentContractValue; // How much money player is getting upon finishing the level. Can be reduced if destructible objects are damaged/destroyed
    public static float levelTimer; // Timer for level
    public static int startingNumberOfFlies; // Starting Number of flies for the level
    public static int maxNumberOfSpecials;  // max number of special enemies in a level
    public static float enemyHealthModifier;
    public static float enemySpeedModifier;
    public static float spawnChanceModifier;
    public static float moneyPenaltyModifier;

    // Number of enemies remaining on level
    public static int flyCount;
    public static int spiderCount;
    public static int scorpionCount;

    // Kill count
    public static int flyKillCount;
    public static int spiderKillCount;
    public static int scorpionKillCount;

    // End level stats
    public static float levelBonus; // Bonus money for finishing level before timer ran out
    public static float levelPenalty; // Money lost for not finishing level before timer ran out
    public static float totalDamageCosts; // Amount of money lost due to destroyed objects

    // *******************************


    private string difficulty;


    void Awake()
    {
        SetupLevel();
    }

    void Update()
    {
        UpdateHUD();
    }

    private void SetupLevel()
    {
        // SetDifficulty(); USE THIS WHEN MAIN MENU IS RUNNING

        difficulty = (hardMode) ? "hard" : "normal"; // temporary. for testing purposes. TO DELETE

        if (difficulty == "normal")
            NormalMode();
        else if (difficulty == "hard")
            HardMode();
    }

    private void SetDifficulty()
    {
        difficulty = PlayerPrefs.GetString("difficulty");

        // null check on difficulty. default to normal
        if (difficulty == null)
        {
            difficulty = "normal";
        }
        Debug.Log(difficulty); // TO DELETE
    }

    private void NormalMode()
    {
        currentContractValue = contractValueNormal;
        levelTimer = levelTimerNormal;
        startingNumberOfFlies = totalNumberOfFliesNormal;
        maxNumberOfSpecials = maxNumberOfSpecialsNormal;
        enemyHealthModifier = enemyHealthModifierNormal;
        enemySpeedModifier = enemySpeedModifierNormal;
        spawnChanceModifier = spawnChanceModifierNormal;
        moneyPenaltyModifier = moneyPenaltyModifierNormal;
    }

    private void HardMode()
    {
        currentContractValue = contractValueHard;
        levelTimer = levelTimerHard;
        startingNumberOfFlies = totalNumberOfFliesHard;
        maxNumberOfSpecials = maxNumberOfSpecialsHard;
        enemyHealthModifier = enemyHealthModifierHard;
        enemySpeedModifier = enemySpeedModifierHard;
        spawnChanceModifier = spawnChanceModifierHard;
        moneyPenaltyModifier = moneyPenaltyModifierHard;
    }

    private void UpdateHUD()
    {
        moneyDisplay.text = String.Format("{0:.00}", currentContractValue);
        flyCounterDisplay.text = flyCount.ToString();
    }

    // If level was completed before timer ended, add bonus money based on how much time was left
    public static void CalculateBonus(int timeRemaining)
    {
        currentContractValue += timeRemaining;
    }

    // If level ended when timer ran out, deduct money based on how many flies that are alive
    public static void CalculatePenalty()
    {
        currentContractValue -= flyCount * 4; // each fly will reduce contract value by 4$
    }

    public static void LevelOver(int timeRemaining)
    {
        if (flyCount == 0)
        {
            CalculateBonus(timeRemaining);
        }
        else
        {
            CalculatePenalty();
        }

        // TODO: Display the end level screen with stats
    }


    // Contract value increase/decrease functions
    public static void IncreaseContractValue(float bonus)
    {
        currentContractValue += bonus;
    }
    public static void ReduceContractValue(float penalty)
    {
        currentContractValue -= penalty * moneyPenaltyModifier;
        totalDamageCosts += penalty;
    }


    // Enemy counter increment/decrement functions
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


    // Enemy kill count increment/decrement functions
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
