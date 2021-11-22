using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;

    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        health *= GameManager.enemyHealthModifier; // Depending on difficulty, health will be adjusted
    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float amount)
    {
        if (gameObject.CompareTag("Enemy"))
        {
            _audioManager.Play("DamageHit");
        }

        health -= amount;
    }

    public void Die()
    {
        FlyRelativeMovmement frm = GetComponent<FlyRelativeMovmement>();
        if (frm)
            frm.SetFlyMode(FlyMode.DEATH);

        SpiderNavigator spn = GetComponent<SpiderNavigator>();
        if (spn)
            spn.SetDead();

        ScorpionNavigator scn = GetComponent<ScorpionNavigator>();
        if (scn)
            scn.SetDead();
    }
}
