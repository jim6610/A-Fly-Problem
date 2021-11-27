using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SprayBottle : MonoBehaviour
{
    [Header("Weapon Parameters")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private int totalAmmo = 20;

    [Header("Effects")]
    [SerializeField] private ParticleSystem particles;

    [Header("Animation")]
    public Animator animator;

    private AudioManager audioManager;
    private GameObject weaponManager;
    private GameObject selectionManager;
    private Text ammoDisplay;
    private CapsuleCollider areaOfEffect;

    private Camera fpsCam;
    private float nextTimeToFire;
    private int currentAmmoClip;
    private bool isReloading = false;

    private bool CanFire => Input.GetButton("Fire1") && Time.time >= nextTimeToFire;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        weaponManager = GameObject.Find("WeaponHolder");
        selectionManager = GameObject.Find("SelectionManager");
        ammoDisplay = GameObject.Find("Ammo").GetComponent<Text>();
        fpsCam = Camera.main;
        currentAmmoClip = totalAmmo;
        areaOfEffect = gameObject.GetComponent<CapsuleCollider>();
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    void Update()
    {
        // If able to fire and ammo clip is not empty, fire weapon
        if (CanFire && currentAmmoClip != 0)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Shoot();
        }

        // If player tries to fire an empty weapon, empty clip sound effect will play
        if (CanFire && currentAmmoClip == 0)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            audioManager.Play("RifleClipEmpty");
        }

        // Update ammo display on HUD
        ammoDisplay.text = currentAmmoClip + " | 0";
    }


    /// Weapon firing logic
    void Shoot()
    {
        audioManager.Play("Spray");

        currentAmmoClip--;

        animator.ResetTrigger("Shoot");
        animator.SetTrigger("Shoot");
        particles.Play();

        areaOfEffect.enabled = true;
        StartCoroutine(ShootCoroutine());

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<FlySprayDebuff>().Sprayed();
            Debug.Log("hit");
        }
    }

    IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        areaOfEffect.enabled = false;
    }
}
