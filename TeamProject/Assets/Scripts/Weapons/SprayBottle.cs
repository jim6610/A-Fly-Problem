using UnityEngine;
using System.Collections;


/// Rifle ranged weapon
/// TODO Code not DRY, shares almost all game variables/logic with FlySwatter, can probably use inheritance here
public class SprayBottle : MonoBehaviour
{
    [Header("Weapon Parameters")]
    [SerializeField] private float damage = 4f;
    [SerializeField] private float impactForce = 75f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float reloadTime = 6;
    [SerializeField] private int ammoRemaining = 15;

    [Header("Effects")]
    [SerializeField] private ParticleSystem particles;
    private AudioManager audioManager;
    private GameObject weaponManager;
    private GameObject selectionManager;
    public Animator animator;

    private Camera fpsCam;
    private float nextTimeToFire;
    private int currentAmmoClip;
    private bool isReloading = false;
    private CapsuleCollider areaOfEffect;

    private bool CanFire => Input.GetButton("Fire1") && Time.time >= nextTimeToFire;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        weaponManager = GameObject.Find("WeaponHolder");
        selectionManager = GameObject.Find("SelectionManager");
        fpsCam = Camera.main;
        currentAmmoClip = ammoRemaining;
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

    void OnCollisionEnter(Collision collision)
    {
        //Output the Collider's GameObject's name
        Debug.Log(collision.collider.name);
    }

    IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        areaOfEffect.enabled = false;
    }
}
