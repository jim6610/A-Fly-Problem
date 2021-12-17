using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    [Header("Shop")]
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject[] buyButtons;
    [SerializeField] private GameObject[] equipdButtons;

    [SerializeField] private Text ammount;
    [SerializeField] private Text equipWarn;
    [SerializeField] private Text moneyWarn;

    [Header("Override Shop")]
    public float customMoney = 0;
    public bool clearInventory = false;
    public static bool overrideExecuted = false;

    private GameManager gameManager;
    private AudioManager audioManager;

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
        gameManager = FindObjectOfType<GameManager>();

        if (!overrideExecuted && customMoney != 0)
        {
            Debug.Log("You are overriding the shop and using a custom amount of money");
            money = customMoney;
        }
        else
        {
            if (PlayerPrefs.HasKey("money"))
            {
                money = PlayerPrefs.GetFloat("money");
            }
            else
            {
                money = 0f;
                PlayerPrefs.SetFloat("money", money);
            }
        }

        if (!overrideExecuted && clearInventory)
        {
            Debug.Log("You are overriding the shop and clearing the inventory");
            inventory = "";
        }
        else
        {
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
        }
   
        loadout = "";
        loadoutCounter = 0;

        overrideExecuted = true;
    }


    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
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
                audioManager.Play("Buy");
            }

            else if (item == "bat")
            {
                buyButtons[1].SetActive(false);
                equipdButtons[1].SetActive(true);
                audioManager.Play("Buy");
            }

            else if (item == "spray")
            {
                buyButtons[2].SetActive(false);
                equipdButtons[2].SetActive(true);
                audioManager.Play("Buy");
            }

            else if (item == "gun")
            {
                buyButtons[3].SetActive(false);
                equipdButtons[3].SetActive(true);
                audioManager.Play("Buy");
            }

            else if (item == "rifle")
            {
                buyButtons[4].SetActive(false);
                equipdButtons[4].SetActive(true);
                audioManager.Play("Buy");
            }

            else if (item == "flame")
            {
                buyButtons[5].SetActive(false);
                equipdButtons[5].SetActive(true);
                audioManager.Play("Buy");
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
        audioManager.Play("Equip");
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
        money = PlayerPrefs.GetFloat("money", 0);
        inventory = PlayerPrefs.GetString("inventory", "");

        this.gameObject.SetActive(false);
        levelSelect.SetActive(true);
    }


    public void startLevel()
    {
        if(loadout.Length > 0 && loadoutCounter > 0 && loadoutCounter <= 4)
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
