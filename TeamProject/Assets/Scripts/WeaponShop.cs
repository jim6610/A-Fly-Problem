using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShop : MonoBehaviour
{
    [SerializeField] private GameObject weaponHolder;
    
    private WeaponSwitching weaponSwitching; // can be removed once weapon shop is in main menu and not in level
    private InventoryManager inventoryManager;
    
    private bool firstWeapon = true;

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        weaponSwitching = FindObjectOfType<WeaponSwitching>();
        
        Time.timeScale = 0;
    }
    
    public void BuyWeapon(GameObject a)
    {
        if(firstWeapon)
        {
            Instantiate(a, weaponHolder.transform);
            firstWeapon = false;
        }
        else
        {
            Instantiate(a, weaponHolder.transform).SetActive(false);
        }
    }

    public void CloseShop()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
        
        // TO REMOVE: to fix null reference bug. can be removed once weapon shop is in main menu and not in level. SelectWeapon() needs to be called to set an active weapon.
        // Normally this is called in Start() function of WeaponSwitching but because WeaponShop is in the level, the player doesn't start with any weapons and no active weapon can be set. 
        inventoryManager.PopulateInventory();
        weaponSwitching.SelectWeapon(); 
    }
}
