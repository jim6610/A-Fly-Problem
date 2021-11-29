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
    [SerializeField] private int flyCapacityNormal;
    [SerializeField] private float flySpawnRateNormal;
    [SerializeField] private int maxNumberOfSpecialsNormal;
    [SerializeField] private float specialSpawnRateNormal;
    [SerializeField] private float enemyHealthModifierNormal = 1.0f;
    [SerializeField] private float enemySpeedModifierNormal = 1.0f;
    [SerializeField] private float spawnChanceModifierNormal = 1.0f;
    [SerializeField] private float moneyPenaltyModifierNormal = 1.0f;
    [SerializeField] private float ceilingHeightNormal;

    [Header("Hard Mode Variables")]
    [SerializeField] private float contractValueHard;
    [SerializeField] private float levelTimerHard;
    [SerializeField] private int totalNumberOfFliesHard;
    [SerializeField] private int flyCapacityHard;
    [SerializeField] private float flySpawnRateHard;
    [SerializeField] private int maxNumberOfSpecialsHard;
    [SerializeField] private float specialSpawnRateHard;
    [SerializeField] private float enemyHealthModifierHard;
    [SerializeField] private float enemySpeedModifierHard;
    [SerializeField] private float spawnChanceModifierHard;
    [SerializeField] private float moneyPenaltyModifierHard;
    [SerializeField] private float ceilingHeightHard;

    [Header("HUD Text Objects")]
    [SerializeField] private Text moneyDisplay;
    [SerializeField] private Text flyCounterDisplay;

    // *** Global game variables ***

    // Difficulty settings
    public static float currentContractValue; // How much money player is getting upon finishing the level. Can be reduced if destructible objects are damaged/destroyed
    public static float initialContractValue; // initial contract value 
    public static float levelTimer; // Timer for level
    public static int startingNumberOfFlies; // Starting Number of flies for the level
    public static int flyCapacity; // max number of flies in a level at once
    public static float flySpawnRate; // how often a fly spawns in the level
    public static int maxNumberOfSpecials;  // max number of special enemies in a level
    public static float specialSpawnRate; // how often a special spawns in the level
    public static float enemyHealthModifier;
    public static float enemySpeedModifier;
    public static float spawnChanceModifier;
    public static float moneyPenaltyModifier;
    public static float ceilingHeight;

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
        initialContractValue = contractValueNormal;
        levelTimer = levelTimerNormal;
        startingNumberOfFlies = totalNumberOfFliesNormal;
        flyCapacity = flyCapacityNormal;
        flySpawnRate = flySpawnRateNormal;
        maxNumberOfSpecials = maxNumberOfSpecialsNormal;
        specialSpawnRate = specialSpawnRateNormal;
        enemyHealthModifier = enemyHealthModifierNormal;
        enemySpeedModifier = enemySpeedModifierNormal;
        spawnChanceModifier = spawnChanceModifierNormal;
        moneyPenaltyModifier = moneyPenaltyModifierNormal;
    }

    private void HardMode()
    {
        currentContractValue = contractValueHard;
        initialContractValue = contractValueHard;
        levelTimer = levelTimerHard;
        startingNumberOfFlies = totalNumberOfFliesHard;
        flyCapacity = flyCapacityHard;
        flySpawnRate = flySpawnRateHard;
        maxNumberOfSpecials = maxNumberOfSpecialsHard;
        specialSpawnRate = specialSpawnRateHard;
        enemyHealthModifier = enemyHealthModifierHard;
        enemySpeedModifier = enemySpeedModifierHard;
        spawnChanceModifier = spawnChanceModifierHard;
        moneyPenaltyModifier = moneyPenaltyModifierHard;
    }

    private void UpdateHUD()
    {
        moneyDisplay.text = String.Format("{0:.00}", currentContractValue);
        flyCounterDisplay.text = (startingNumberOfFlies - flyKillCount).ToString();
    }

    // If level was completed before timer ended, add bonus money based on how much time was left
    public static void CalculateBonus(int timeRemaining)
    {
        levelBonus = timeRemaining;
    }

    // If level ended when timer ran out, deduct money based on how many flies that are alive
    public static void CalculatePenalty()
    {
        levelPenalty = (startingNumberOfFlies - flyKillCount) * 4; // each fly will reduce contract value by 4$
    }

    public static void LevelOver(int timeRemaining)
    {
        // Pause the game
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;

        // Activate Level Complete Object
        GameObject hud = GameObject.Find("HUD").gameObject;
        GameObject lcScreen = hud.transform.Find("Level Complete").gameObject;
        lcScreen.SetActive(true);

        // Calm down music
        GameObject.Find("BackgroundMusic").gameObject.GetComponent<AudioSource>().volume = 0.2f;
        GameObject.Find("BackgroundMusic").gameObject.GetComponent<AudioSource>().pitch = 0.85f;


        //Success
        if (GameManager.flyKillCount == GameManager.startingNumberOfFlies)
        {
            CalculateBonus(timeRemaining);
            lcScreen.transform.Find("Heading").gameObject.GetComponent<Text>().text = "All flies eradicated!";
            // Play SuccessJig
            GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("Victory");

        }
        //Failure
        else
        {
            CalculatePenalty();
            lcScreen.transform.Find("Heading").gameObject.GetComponent<Text>().text = "Time is up...";
            // Play FailureJig
            GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("Failure");
        }

        //Update Values
        lcScreen.transform.Find("SheetAndButtons/Sheet/ContractAmount").gameObject.GetComponent<Text>().text = "$" + (initialContractValue);
        lcScreen.transform.Find("SheetAndButtons/Sheet/TimeAmount").gameObject.GetComponent<Text>().text = "$" + (levelBonus);
        lcScreen.transform.Find("SheetAndButtons/Sheet/FliesAmount").gameObject.GetComponent<Text>().text = "- $" + (levelPenalty);
        lcScreen.transform.Find("SheetAndButtons/Sheet/DamageAmount").gameObject.GetComponent<Text>().text = "- $" + (totalDamageCosts);
        lcScreen.transform.Find("SheetAndButtons/Sheet/TotalAmount").gameObject.GetComponent<Text>().text = "$" + (initialContractValue - levelPenalty + levelBonus - totalDamageCosts);
        lcScreen.transform.Find("SheetAndButtons/Sheet/SpiderAmount").gameObject.GetComponent<Text>().text = "Spiders Killed: " + spiderKillCount;
        lcScreen.transform.Find("SheetAndButtons/Sheet/ScorpionAmount").gameObject.GetComponent<Text>().text = "Scorpions Killed: " + scorpionKillCount;
        lcScreen.transform.Find("SheetAndButtons/Sheet/Tip").gameObject.GetComponent<Text>().text = "Tip from the owner: \n \n" + RandomTip();


        // TODO: add (currentContractValue - levelPenalty + levelBonus - totalDamageCosts) to player money
        float money = PlayerPrefs.GetFloat("money");
        money += (currentContractValue - levelPenalty + levelBonus - totalDamageCosts);
        PlayerPrefs.SetFloat("money", money);
        PlayerPrefs.Save();
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

    // Return a tip from the tip list
    public static String RandomTip()
    {
        String[] tipList = {
            "Spraying a fly multiple times in a row will make it stay still for a few seconds.",
            "Try not to break things, cost in damages will come out of your pocket.",
            "Shutting off the breakers turn off all the lights, lowering your visibilty but decrease speed of the flies.",
            "I will task myself with eriting more tips, but if you have ideas throw them in here."
        };
        return tipList[UnityEngine.Random.Range(0, tipList.Length)];
    }

    // Enemy counter increment/decrement functions
    public static Action IncrementFlyCount = () => flyCount++;
    public static Action IncrementSpiderCount = () => spiderCount++;
    public static Action IncrementScorpionCount = () => scorpionCount++;
    public static Action DecrementFlyCount = () => flyCount--;
    public static Action DecrementSpiderCount = () => spiderCount--;
    public static Action DecrementScorpionCount = () => scorpionCount--;


    // Enemy kill count increment/decrement functions
    public static Action IncrementFlyKillCount = () => flyKillCount++;
    public static Action IncrementSpiderKillCount = () => spiderKillCount++;
    public static Action IncrementScorpionKillCount = () => scorpionKillCount++;
}