// Part of the script is from the Destructible Props Pack by Vitaly
// https://assetstore.unity.com/packages/3d/props/destructible-props-pack-27379

using UnityEngine;
using UnityEngine.UI;

public class Destructible : MonoBehaviour {
    [SerializeField] private float initialHealth;
    [SerializeField] private float monetaryValue;
    [SerializeField] private bool isHanging;
    [SerializeField] private float breakForce; // How easily the object breaks
    [SerializeField] private float explosionForce = 200; // force added to every piece of the broken object
    [SerializeField] private float destroyedPiecesRotation = 20; // Rotation force added to every chunk when it explodes
    [SerializeField] private bool destroyAftertime = true; // if true, then destroyed pieces will be removed after a set time
    [SerializeField] private float time = 5; // time before destroyed pieces will be destroyed from the scene
    [SerializeField] private GameObject[] destroyedPieces;
    [SerializeField] private GameObject[] hidePieces; // list of the objects that will be hidden after the object breaks
    [SerializeField] private GameObject FX; // special effect
    [SerializeField] private GameObject deductionEffect;
    private RectTransform deductionPosition;

    private float currentHealth;
    private void Start()
    {
        currentHealth = initialHealth;

        if (GameObject.Find("Deduction").GetComponent<RectTransform>())
        {
            deductionPosition = GameObject.Find("Deduction").GetComponent<RectTransform>();
        }


        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.1f); // Randomize shatter sound
        }
    }


    public void TakeDamage(float amount)
    {
        if (currentHealth > 0)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                Shatter();
            }
        }
    }

    public void LoseMoney()
    {
        GameManager.ReduceContractValue(monetaryValue);

        if (deductionPosition)
        {
            deductionEffect.GetComponent<Text>().text = "-" + monetaryValue.ToString();
            Instantiate(deductionEffect, deductionPosition.transform);
        }

    }

    void Shatter()
    {
        LoseMoney();

        // if there were any 
        if (hidePieces.Length != 0)
        {
            foreach (GameObject hidingObj in hidePieces)
            {
                hidingObj.SetActive(false);
            }
        }

        if (FX)
        {
            FX.SetActive(true);
        }
        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().Play();
        }
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        foreach (GameObject chunk in destroyedPieces)
        {
            chunk.SetActive(true);
            chunk.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * -explosionForce);
            chunk.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.forward * -destroyedPiecesRotation * Random.Range(-5f, 5f));
            chunk.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * -destroyedPiecesRotation * Random.Range(-5f, 5f));
            if (destroyAftertime)
            {
                Invoke("DestructObject", time);
            }
        }
    }

    void DestructObject()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        Vector3 impactForce = other.relativeVelocity;

        if (isHanging)
        {
            this.GetComponent<Rigidbody>().useGravity = true;
        }

        if (other.relativeVelocity.magnitude > breakForce)
        {
            if (other.collider.CompareTag("Enemy"))
            {
                other.collider.GetComponent<EnemyHealth>().TakeDamage(initialHealth);
                this.TakeDamage(other.collider.GetComponent<EnemyHealth>().health);
            }
            else if (other.collider.CompareTag("Destructible"))
            {
                other.collider.GetComponent<Destructible>().TakeDamage(initialHealth);
                this.TakeDamage(other.collider.GetComponent<Destructible>().initialHealth);
            }
            else
            {
                this.TakeDamage(initialHealth);
            }
        }
    }
}
