using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponShop : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    private string shoppingList;

    private void Start()
    {
        shoppingList = "";
    }

    public void BuyWeapon(string a)
    {
        shoppingList += a + ',';
    }

    public void returnToMain()
    {
        shoppingList = "";
        this.gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }


    public void startLevel()
    {
        if(shoppingList.Length > 0)
        {
            shoppingList = shoppingList.Remove(shoppingList.Length - 1, 1);
        }
        PlayerPrefs.SetString("items", shoppingList);
        PlayerPrefs.Save();

        SceneManager.LoadScene(1);
    }

}
