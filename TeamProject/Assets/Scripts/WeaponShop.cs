using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShop : MonoBehaviour
{
    [SerializeField] private GameObject weaponHolder;
    
    private InventoryManager inventoryManager;
    private bool firstWeapon = true;

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
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
        inventoryManager.PopulateInventory();
        gameObject.SetActive(false);
    }
}
