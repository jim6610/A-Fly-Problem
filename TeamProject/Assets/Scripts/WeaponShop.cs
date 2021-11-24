using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject[] buyButtons;
    [SerializeField] private GameObject[] equipdButtons;

    [SerializeField] private Text ammount;
    [SerializeField] private Text equipWarn;
    [SerializeField] private Text moneyWarn;

    private float timeToAppear = 2f;
    private float timeWhenDisappearEquip;
    private float timeWhenDisappearMoney;

    private string loadout;
    private string inventory;
    private float money;
    private int scene;

    private int loadoutCounter;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("money"))
        {
            money = PlayerPrefs.GetFloat("money");
        }
        else
        {
            money = 10000f;
            PlayerPrefs.SetFloat("money", money);
        }

        if (PlayerPrefs.HasKey("inventory"))
        {
            inventory = PlayerPrefs.GetString("inventory");
        }
        else
        {
            inventory = "";
            PlayerPrefs.SetString("inventory", inventory);
        }
        PlayerPrefs.Save();


        loadout = "";
        loadoutCounter = 0;
    }


    private void Start()
    {
        checkInventory();
    }

    private void Update()
    {
        ammount.text = money.ToString();

        if (equipWarn.enabled && (Time.time >= timeWhenDisappearEquip))
        {
            equipWarn.enabled = false;
        }

        if (moneyWarn.enabled && (Time.time >= timeWhenDisappearMoney))
        {
            moneyWarn.enabled = false;
        }
    }

    public void setScene(int i)
    {
        scene = i;
    }

    public void buyWeapon(string a)
    {

        string[] splitParams = a.Split(',');

        string item = splitParams[0];
        int price = int.Parse(splitParams[1]);

        if(price <= money)
        {
            inventory += item + ',';
            money -= price;

            if (item == "swatter")
            {
                buyButtons[0].SetActive(false);
                equipdButtons[0].SetActive(true);
            }

            else if (item == "bat")
            {
                buyButtons[1].SetActive(false);
                equipdButtons[1].SetActive(true);
            }

            else if (item == "spray")
            {
                buyButtons[2].SetActive(false);
                equipdButtons[2].SetActive(true);
            }

            else if (item == "gun")
            {
                buyButtons[3].SetActive(false);
                equipdButtons[3].SetActive(true);
            }

            else if (item == "rifle")
            {
                buyButtons[4].SetActive(false);
                equipdButtons[4].SetActive(true);
            }

            else if (item == "flame")
            {
                buyButtons[5].SetActive(false);
                equipdButtons[5].SetActive(true);
            }

        }
        else
        {
            moneyWarn.enabled = true;
            timeWhenDisappearMoney = Time.time + timeToAppear;
        }
    }

    public void equipWeapon(string a)
    {
        loadout += a + ',';
        loadoutCounter++;
    }

    public void unequipWeapon(string a)
    {
        string temp = a + ",";
        loadout = loadout.Replace(temp, "");
        loadoutCounter--;
    }

    public void returnToLevelSelect()
    {
        loadout = "";
        loadoutCounter = 0;
        money = PlayerPrefs.GetFloat("money");
        inventory = PlayerPrefs.GetString("inventory");

        this.gameObject.SetActive(false);
        levelSelect.SetActive(true);
    }


    public void startLevel()
    {
        if(loadout.Length > 0 && loadoutCounter > 0 && loadoutCounter <= 3)
        {
            loadout = loadout.Remove(loadout.Length - 1, 1);
            PlayerPrefs.SetString("loadout", loadout);
            PlayerPrefs.SetString("inventory", inventory);
            PlayerPrefs.SetFloat("money", money);
            PlayerPrefs.Save();
            SceneManager.LoadScene(scene);
        }
        else
        {
            equipWarn.enabled = true;
            timeWhenDisappearEquip = Time.time + timeToAppear;
        }
    }

    public void checkInventory()
    {
        if(inventory.Contains("swatter,"))
        {
            buyButtons[0].SetActive(false);
            equipdButtons[0].SetActive(true);
        }

        if (inventory.Contains("bat,"))
        {
            buyButtons[1].SetActive(false);
            equipdButtons[1].SetActive(true);
        }

        if (inventory.Contains("spray,"))
        {
            buyButtons[2].SetActive(false);
            equipdButtons[2].SetActive(true);
        }

        if (inventory.Contains("gun,"))
        {
            buyButtons[3].SetActive(false);
            equipdButtons[3].SetActive(true);
        }

        if (inventory.Contains("rifle,"))
        {
            buyButtons[4].SetActive(false);
            equipdButtons[4].SetActive(true);
        }

        if (inventory.Contains("flame,"))
        {
            buyButtons[5].SetActive(false);
            equipdButtons[5].SetActive(true);
        }
    }

}
