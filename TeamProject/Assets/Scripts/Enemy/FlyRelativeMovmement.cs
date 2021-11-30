using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlyMode
{
    TRAVELLING = 0,
    FLOOR_LANDING = 1,
    WALL_LANDING = 2,
    STATIONARY = 3,
    DEATH = 4
}

[RequireComponent(typeof(Rigidbody))]
public class FlyRelativeMovmement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private float thrustForce;
    [SerializeField]
    private float targetHeightRange;
    [SerializeField]
    private float ceilingAwareness;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float stationaryWaitTime;

    private Transform navigatorTransform;
    private FlyNavigator flyNavigator;
    private FlyMode flyMode;
    private BoxCollider collider;
    private float waitTimer;
    private float takeOffTime;
    

    private float targetHeight;
    private float targetHeightMin = 0.25f;
    private float targetHeightTimer;
    private float targetHeightTime;

    private Animator animator;

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        rb = GetComponent<Rigidbody>();
        flyNavigator = GetComponentInParent<FlyNavigator>();
        navigatorTransform = flyNavigator.gameObject.transform;
        flyMode = FlyMode.TRAVELLING;
        collider = GetComponent<BoxCollider>();
        waitTimer = 0.0f;
        takeOffTime = 0.5f;
        targetHeightTime = 1.75f;
        targetHeight = Random.Range(targetHeightMin, targetHeightRange);
        animator = GetComponentInChildren<Animator>();
    }

    private void FlyRoutine()
    {
        animator.SetBool("animFly", true);
        animator.SetBool("animScratch", false);
        targetHeightTimer += Time.deltaTime;
        if(targetHeightTimer>targetHeightTime)
        {
            targetHeight = Random.Range(targetHeightMin, targetHeightRange);
            targetHeightTimer = 0.0f;
        }
        bool normalTrajectory = true;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit))
        {
            if (hit.distance < ceilingAwareness)
            {
                rb.AddRelativeForce(Vector3.down * thrustForce);
                normalTrajectory = false;
            }
            else
                normalTrajectory = true;
        }

        //print("target height: " + targetHeight);
        if (normalTrajectory)
        {
            if (Mathf.Abs(targetHeight - transform.localPosition.y) > 0.5f)
                rb.AddRelativeForce(Vector3.up * thrustForce * (targetHeight - transform.localPosition.y));
            else
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void LandingRoutine()
    {
        animator.SetBool("animFly", true);
        animator.SetBool("animScratch", false);
        if (flyNavigator.PlayerInRange(transform))
        {
            flyNavigator.GetTargetAimlessly();
            flyMode = FlyMode.TRAVELLING;
        }
        if (rb.velocity.y > 0)
            rb.AddRelativeForce(Vector3.down * thrustForce);
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.distance > collider.extents.z + 0.05f)
                    rb.AddRelativeForce((Vector3.up * -rb.velocity.y) + (hit.distance * Vector3.down));
                else
                    flyMode = FlyMode.STATIONARY;
            }
            else
                rb.AddRelativeForce(Vector3.up * -rb.velocity.y);
        }
    }

    void StationaryRoutine()
    {
        animator.SetBool("animFly", false);
        animator.SetBool("animScratch", true);
        waitTimer += Time.deltaTime;

        bool playerWasInRange = flyNavigator.PlayerInRange(transform);
        if (waitTimer > stationaryWaitTime || playerWasInRange)
        {
            
            rb.AddRelativeForce(Vector3.up * thrustForce);
            animator.SetBool("animFly", true);
            animator.SetBool("animScratch", false);

            if (waitTimer > stationaryWaitTime + takeOffTime || playerWasInRange)
            {
                waitTimer = 0.0f;
                flyNavigator.GetTargetAimlessly();
                flyMode = FlyMode.TRAVELLING;
            }
        }
    }



    void DeathRoutine()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                Destroy(navigatorTransform.gameObject);
                Destroy(this.gameObject);
            }
        }
    }



    void Update()
    {
        switch (flyMode)
        {
            case FlyMode.TRAVELLING:
                FlyRoutine();
                break;
            case FlyMode.FLOOR_LANDING:
                LandingRoutine();
                break;
            case FlyMode.WALL_LANDING:
                break;
            case FlyMode.STATIONARY:
                StationaryRoutine();
                break;
            case FlyMode.DEATH:
                DeathRoutine();
                break;

        }

    }

    public void SetFlyMode(FlyMode flyMode)
    {
        if(flyMode == FlyMode.DEATH)
        {
            animator.SetBool("animFly", false);
            animator.SetBool("animScratch", false);
            animator.SetBool("animDeath", true);
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            waitTimer = 0.0f;
            transform.parent = null;
        }
        this.flyMode = flyMode;
    }

    public FlyMode GetFlyMode()
    {
        return flyMode;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }
}
