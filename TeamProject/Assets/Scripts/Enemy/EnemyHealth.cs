using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;

    private AudioManager _audioManager;
    private bool dead;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        health *= GameManager.enemyHealthModifier; // Depending on difficulty, health will be adjusted
        dead = false;
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
        if (!dead)
        {
            FlyRelativeMovmement frm = GetComponent<FlyRelativeMovmement>();
            if (frm)
            {
                frm.SetFlyMode(FlyMode.DEATH);
                GameManager.DecrementFlyCount();
            }

            SpiderNavigator spn = GetComponent<SpiderNavigator>();
            if (spn)
            {
                spn.SetDead();
                GameManager.DecrementSpiderCount();
            }

            ScorpionNavigator scn = GetComponent<ScorpionNavigator>();
            if (scn)
            {
                scn.SetDead();
                GameManager.DecrementScorpionCount();
            }
            dead = true;
        }
    }
}
