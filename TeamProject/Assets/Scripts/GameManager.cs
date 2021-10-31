using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float contractValue; // How much money player is getting upon finishing the level. Can be reduced if destructible objects are damaged/destroyed

    // Start is called before the first frame update
    void Start()
    {
        contractValue = 100;
    }

    // Update is called once per frame
    void Update()
    {
        print("Contract Value: " + contractValue);
    }

    public static void ReduceContractValue(float reduction)
    {
        contractValue -= reduction;
    }
}
