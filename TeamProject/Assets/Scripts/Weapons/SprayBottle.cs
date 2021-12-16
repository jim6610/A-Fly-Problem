using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SprayBottle : Weapon
{
    [Header("Weapon Parameters")]
    [SerializeField] private int totalAmmo = 20;

    [Header("Effects")]
    [SerializeField] private ParticleSystem particles;
    
    private CapsuleCollider _areaOfEffect;
    private int _currentAmmoClip;
    
    protected override string fireSoundTag => "Spray";
    private string emptyClipSoundTag => "RifleClipEmpty";

    private static readonly int ShootTrigger = Animator.StringToHash("Shoot");
    private static readonly int Reloading = Animator.StringToHash("Reloading");

    private void Start()
    {
        _currentAmmoClip = totalAmmo;
        _areaOfEffect = gameObject.GetComponent<CapsuleCollider>();
    }
    
    void OnEnable()
    {
        animator.SetBool(Reloading, false);
    }
    
    public override void Equipped() {}

    void Update()
    {
        // If able to fire and ammo clip is not empty, fire weapon
        if (CanFire && _currentAmmoClip != 0)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            Fire();
        }

        // If player tries to fire an empty weapon, empty clip sound effect will play
        if (CanFire && _currentAmmoClip == 0)
        {
            nextTimeToFire = Time.time + 1 / fireRate;
            audioManager.Play(emptyClipSoundTag);
        }

        // Update ammo display on HUD
        ammoDisplay.text = _currentAmmoClip + " | 0";
    }

    /// Weapon firing logic
    protected override void Fire()
    {
        audioManager.Play(fireSoundTag);

        _currentAmmoClip--;

        animator.ResetTrigger(ShootTrigger);
        animator.SetTrigger(ShootTrigger);
        particles.Play();

        _areaOfEffect.enabled = true;
        StartCoroutine(ShootCoroutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<FlyDebuff>().Sprayed();
        }
    }

    IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        _areaOfEffect.enabled = false;
    }
}
