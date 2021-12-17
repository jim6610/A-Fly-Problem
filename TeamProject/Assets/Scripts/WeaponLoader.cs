using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    private string[] items;
    private bool firstWeapon = true;

    private void Awake()
    {
        items = PlayerPrefs.GetString("loadout", "").Split(',');
        loadWeapons();
    }

    private void loadWeapons()
    {
        if(items.Length > 0)
        {
            foreach(string x in items)
            {
                if(x == "swatter")
                {
                    if (firstWeapon)
                    {
                        Instantiate(weapons[0], this.gameObject.transform);
                        firstWeapon = false;
                    }
                    else
                    {
                        Instantiate(weapons[0], this.gameObject.transform).SetActive(false);
                    }
                }

                else if (x == "bat")
                {
                    if (firstWeapon)
                    {
                        Instantiate(weapons[1], this.gameObject.transform);
                        firstWeapon = false;
                    }
                    else
                    {
                        Instantiate(weapons[1], this.gameObject.transform).SetActive(false);
                    }
                }

                else if (x == "spray")
                {
                    if (firstWeapon)
                    {
                        Instantiate(weapons[2], this.gameObject.transform);
                        firstWeapon = false;
                    }
                    else
                    {
                        Instantiate(weapons[2], this.gameObject.transform).SetActive(false);
                    }
                }

                else if (x == "gun")
                {
                    if (firstWeapon)
                    {
                        Instantiate(weapons[3], this.gameObject.transform);
                        firstWeapon = false;
                    }
                    else
                    {
                        Instantiate(weapons[3], this.gameObject.transform).SetActive(false);
                    }
                }

                else if (x == "rifle")
                {
                    if (firstWeapon)
                    {
                        Instantiate(weapons[4], this.gameObject.transform);
                        firstWeapon = false;
                    }
                    else
                    {
                        Instantiate(weapons[4], this.gameObject.transform).SetActive(false);
                    }
                }

                else if (x == "flame")
                {
                    if (firstWeapon)
                    {
                        Instantiate(weapons[5], this.gameObject.transform);
                        firstWeapon = false;
                    }
                    else
                    {
                        Instantiate(weapons[5], this.gameObject.transform).SetActive(false);
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
