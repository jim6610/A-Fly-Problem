using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelect : MonoBehaviour
{
    public GameObject defaultSwatterWeapon;
    public GameObject pickedSwattedWeapon;
    public GameObject defaultGunWeapon;
    public GameObject pickedGunWeapon;    

    // Start is called before the first frame update
    void Start()
    {        
             
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("swicth to swatter");

            defaultSwatterWeapon.SetActive(false);
            pickedSwattedWeapon.SetActive(true);
            defaultGunWeapon.SetActive(true);
            pickedGunWeapon.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("swicth to gun");

            defaultSwatterWeapon.SetActive(true);
            pickedSwattedWeapon.SetActive(false);
            defaultGunWeapon.SetActive(false);
            pickedGunWeapon.SetActive(true);
        }
    }
}
