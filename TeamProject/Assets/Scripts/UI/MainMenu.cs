using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject splash;


    public void Start()
    {
        Invoke("removeArt", 1);
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
}
