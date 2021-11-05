using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlyMode
{
    TRAVELLING = 0,
    FLOOR_LANDING = 1,
    WALL_LANDING = 2,
    STATIONARY = 3
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
    private Animator animator;

    private float targetHeight;
    private float targetHeightMin = 1.0f;
    private float targetHeightTimer;
    private float targetHeightTime;



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
        targetHeightTime = 1.0f;
        targetHeight = Random.Range(targetHeightMin, targetHeightRange);
        animator = GetComponentInChildren<Animator>();
    }

    private void FlyRoutine()
    {
        animator.SetBool("animFly", true);
        animator.SetBool("animEat", false);
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

        if (normalTrajectory)
        {
            if (Mathf.Abs(targetHeight - transform.position.y) > 0.5f)
                rb.AddRelativeForce(Vector3.up * thrustForce * (targetHeight - transform.position.y));
            else
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void LandingRoutine()
    {
        animator.SetBool("animFly", true);
        animator.SetBool("animEat", false);
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
        animator.SetBool("animEat", true);
        waitTimer += Time.deltaTime;
        bool playerWasInRange = flyNavigator.PlayerInRange(transform);
        if (waitTimer > stationaryWaitTime || playerWasInRange)
        {
            
            rb.AddRelativeForce(Vector3.up * thrustForce);
            animator.SetBool("animFly", true);
            animator.SetBool("animEat", false);
            if (waitTimer > stationaryWaitTime + takeOffTime || playerWasInRange)
            {
                waitTimer = 0.0f;
                flyNavigator.GetTargetAimlessly();
                flyMode = FlyMode.TRAVELLING;
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

        }

    }

    public void SetFlyMode(FlyMode flyMode)
    {
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
