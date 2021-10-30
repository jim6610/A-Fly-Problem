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
    private float targetHeight;
    [SerializeField]
    private float ceilingAwareness;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float stationaryWaitTime;

    private Transform navigatorTransform;
    private FlyNavigator flyNavigator;
    private bool travellingUp;
    private FlyMode flyMode;
    private BoxCollider collider;
    private float waitTimer;
    private float takeOffTime;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        flyNavigator = GetComponentInParent<FlyNavigator>();
        navigatorTransform = flyNavigator.gameObject.transform;
        travellingUp = true;
        flyMode = FlyMode.TRAVELLING;
        collider = GetComponent<BoxCollider>();
        waitTimer = 0.0f;
        takeOffTime = 0.5f;

    }



    private void SwerveRoutine()
    {
        bool normalTrajectory = true;
        if (travellingUp)
        {
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
                if (transform.position.y < navigatorTransform.position.y+ targetHeight)
                    rb.AddRelativeForce(Vector3.up * thrustForce);
                else
                    travellingUp = false;
            }

        }
        else
        {
            if (transform.position.y > navigatorTransform.position.y + targetHeight)
                rb.AddRelativeForce(Vector3.down * thrustForce);
            else
                travellingUp = true;
        }
    }

    void LandingRoutine()
    {
        if (rb.velocity.y > 0)
            rb.AddRelativeForce(Vector3.down * thrustForce);
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.distance > collider.extents.z+0.05f )
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
        waitTimer += Time.deltaTime;
        if(waitTimer>stationaryWaitTime)
        {
            rb.AddRelativeForce(Vector3.up * thrustForce);
            if (waitTimer > stationaryWaitTime + takeOffTime)
            {
                waitTimer = 0.0f;
                flyNavigator.GetNewDestination();
                flyMode = FlyMode.TRAVELLING;
            }
        }
    }



    void Update()
    {
        switch(flyMode)
        {
            case FlyMode.TRAVELLING:
                SwerveRoutine();
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
