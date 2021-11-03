using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private List<Transform> inventorySlots = new List<Transform>(5);

    private WeaponSwitching weaponHolder;

    private int selectionHistory;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color defaultColor;

    void Start()
    {
        weaponHolder = FindObjectOfType<WeaponSwitching>();
        GetChildren();
        
        selectionHistory = weaponHolder.SelectedWeapon;
    }
    
    /// This should only ever be called once at the start of the game (After weapons have been instantiated)
    /// Since the weapons are assigned at runtime I can't load them myself in the start method
    public void PopulateInventory()
    {
        var index = 0;
        foreach (Transform child in weaponHolder.transform)
        {
            // Each Slot has a child where the image is stored
            var slot = inventorySlots[index].GetChild(0);
            // Enable the Images parent GameObject
            slot.gameObject.SetActive(true);
            var slotImage = slot.GetComponent<Image>();
            // Set the weapons icon to the appropriate slot
            slotImage.sprite = child.GetComponent<Image>().sprite;

            index++;
        }

        inventorySlots[selectionHistory].GetComponent<Image>().color = selectedColor;
    }
    
    void Update()
    {
        if (selectionHistory != weaponHolder.SelectedWeapon)
        {
            HandleSelection();
        }
    }

    void HandleSelection()
    {
        var slotImage = inventorySlots[selectionHistory].GetComponent<Image>();
        slotImage.color = defaultColor;
            
        slotImage = inventorySlots[weaponHolder.SelectedWeapon].GetComponent<Image>();
        slotImage.color = selectedColor;

        selectionHistory = weaponHolder.SelectedWeapon;
    }

    void GetChildren()
    {
        foreach (Transform child in transform)
            inventorySlots.Add(child);
    }
}
