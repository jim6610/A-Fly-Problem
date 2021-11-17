using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponShop : MonoBehaviour
{
    [SerializeField] private GameObject levelSelect;
    private string shoppingList;
    private int scene;

    private void Start()
    {
        shoppingList = "";
    }

    public void setScene(int i)
    {
        scene = i;
    }

    public void BuyWeapon(string a)
    {
        shoppingList += a + ',';
    }

    public void returnToLevelSelect()
    {
        shoppingList = "";
        this.gameObject.SetActive(false);
        levelSelect.SetActive(true);
    }


    public void startLevel()
    {
        if(shoppingList.Length > 0)
        {
            shoppingList = shoppingList.Remove(shoppingList.Length - 1, 1);
        }
        PlayerPrefs.SetString("items", shoppingList);
        PlayerPrefs.Save();

        SceneManager.LoadScene(scene);
    }

}
