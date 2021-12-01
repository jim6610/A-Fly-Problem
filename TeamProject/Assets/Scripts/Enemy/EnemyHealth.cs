using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float health;

    private AudioManager _audioManager;
    private bool dead;

    [SerializeField] private GameObject decrementFlyCounterAnimation;
    private RectTransform flyCounterPosition;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        health *= GameManager.enemyHealthModifier; // Depending on difficulty, health will be adjusted
        dead = false;

        if (GameObject.Find("FlyCounterPosition").GetComponent<RectTransform>() != null)
        {
            flyCounterPosition = GameObject.Find("FlyCounterPosition").GetComponent<RectTransform>();
        }

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
                _audioManager.Play("InsectDie");
                GameManager.DecrementFlyCount();
                GameManager.IncrementFlyKillCount();

                if (flyCounterPosition)
                {
                    Instantiate(decrementFlyCounterAnimation, flyCounterPosition.transform);
                }
            }

            SpiderNavigator spn = GetComponent<SpiderNavigator>();
            if (spn)
            {
                spn.SetDead();
                _audioManager.Play("InsectDie");
                GameManager.DecrementSpiderCount();
                GameManager.IncrementSpiderKillCount();
            }

            ScorpionNavigator scn = GetComponent<ScorpionNavigator>();
            if (scn)
            {
                scn.SetDead();
                _audioManager.Play("InsectDie");
                GameManager.DecrementScorpionCount();
                GameManager.IncrementScorpionKillCount();
            }
            dead = true;
        }
    }
}
