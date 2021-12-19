using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject splash;

    private AudioSource codeSuccessfulSound;

    private List<KeyCode> moneyCode = new List<KeyCode>() { KeyCode.D, KeyCode.O, KeyCode.S, KeyCode.H };
    private int keyIndex = 0;

    private SettingsMenu _settingsMenu;

    private bool active;

    public void Start()
    {
        codeSuccessfulSound = GetComponent<AudioSource>();
        _settingsMenu = FindObjectOfType<SettingsMenu>();
        
        Invoke("removeArt", 1);
    }

    private void MoneyCheatListener(KeyCode key)
    {
        Debug.Log(key.ToString());
        if (key == moneyCode[keyIndex]) {
            keyIndex++;

            if (keyIndex >= moneyCode.Count)
            {
                keyIndex = 0;
                codeSuccessfulSound.Play();
                
                PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money", 0) + 5000);
            }
        }
        else
        {
            keyIndex = 0;
        }
    }
    
    private KeyCode DetectKeyPressed() {
        
        foreach(KeyCode key in Enum.GetValues(typeof(KeyCode))) {
            if(Input.GetKeyDown(key)) {
                return key;
            }
        }
        
        return KeyCode.None;
    }

    private void Update()
    {
        if (Input.anyKeyDown)
            MoneyCheatListener(DetectKeyPressed());
    }

    public void removeArt()
    {
        splash.SetActive(false);
    }

    //Main Menu 
    public void loadLevelSelect()
    {
        mainMenu.SetActive(false);
        levelSelect.SetActive(true);
    }

    public void loadCredits()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void closeGame()
    {
        PlayerPrefs.DeleteKey("money");
        PlayerPrefs.DeleteKey("inventory");
        PlayerPrefs.Save();
        Application.Quit();
    }

    //Credits Menu 
    public void loadMainFromCredits()
    {
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    //Level Select Navigation
    public void loadMainFromLevelSelect()
    {
        levelSelect.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void loadShop()
    {
        levelSelect.SetActive(false);
        shopMenu.SetActive(true);
    }

    public void setDifficulty(string diff)
    {
        PlayerPrefs.SetString("difficulty", diff);
    }

    public void ResetStats()
    {
        PlayerPrefs.DeleteKey("money");
        PlayerPrefs.DeleteKey("inventory");
    }
}
