using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShop : MonoBehaviour
{
    [SerializeField] private GameObject weaponHolder;
    private bool firstWeapon = true;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    public void buyWeapon(GameObject a)
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

    public void closeShop()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        this.gameObject.SetActive(false);
    }
}
