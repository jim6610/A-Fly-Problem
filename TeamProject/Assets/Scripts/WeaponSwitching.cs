using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;
    bool weaponsEnabled;

    GameObject activeWeapon;

    void Start()
    {
        SelectWeapon();
        weaponsEnabled = true;
    }

    void Update()
    {
        // If weapons are enabled, you're able to switch between weapons in your inventory
        if (weaponsEnabled)
        {
            SwapWeapon();
        }
    }

    void SwapWeapon()
    {
        int previousSelectedWeapon = selectedWeapon;

        // Scroll wheel control to select weapon
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        // Keys 1-5 to select weapon
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            selectedWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
        {
            selectedWeapon = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && transform.childCount >= 5)
        {
            selectedWeapon = 4;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    public void SelectWeapon()
    {
        int i = 0;

        // Loops over each weapon in weapon holder object and finds a match for the selected weapon which will become the active weapon
        foreach(Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);

                if (transform.childCount > 0)
                {
                    activeWeapon = weapon.gameObject;
                }
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    // Put away weapon if player has grabbed an object
    public void PutAwayCurrentWeapon()
    {
        weaponsEnabled = false;

        if (transform.childCount > 0)
        {
            activeWeapon.SetActive(false);
        }
    }

    // Equip weapon after player has released the grabbed object
    public void EquipPreviousWeapon()
    {
        weaponsEnabled = true;

        if (transform.childCount > 0)
        {
            activeWeapon.SetActive(true);
        }
    }
}
