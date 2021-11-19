using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static float contractValue; // How much money player is getting upon finishing the level. Can be reduced if destructible objects are damaged/destroyed

    [SerializeField] private Text moneyUI;
    
    // Start is called before the first frame update
    void Start()
    {
        contractValue = 1000;
        Debug.Log(PlayerPrefs.GetString("difficulty"));
    }

    // Update is called once per frame
    void Update()
    {
        moneyUI.text = String.Format("{0:.00}", contractValue );
    }

    public static void ReduceContractValue(float reduction)
    {
        contractValue -= reduction;
    }
}
