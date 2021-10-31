using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class FlyNavigator : MonoBehaviour
{
    [SerializeField]
    private float wallAwarenessDistance;
    [Range(1.0f, 99.0f)]
    public float landChance;
    [SerializeField]
    private float landCoolDown;
    [SerializeField]
    private float playerAwarenessDistance = 10.0f;

    private NavMeshAgent agent;
    public GameObject destinationsContainer;
    private BoxCollider[] destinationVolumes;
    private int currentTargetIndex;
    private FlyRelativeMovmement flyRelativeMovement;
    private bool newDestinationAssigned;

    public Transform player;


    float landCooldownTimer = 0.0f;
    bool canLand = false;

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        agent = GetComponent<NavMeshAgent>();
        flyRelativeMovement = GetComponentInChildren<FlyRelativeMovmement>();
        destinationVolumes = destinationsContainer.GetComponentsInChildren<BoxCollider>();
        currentTargetIndex = 0;
        //GetNewDestination(currentTargetIndex);
        GetTargetAimlessly();
        agent.stoppingDistance = 3.0f;
        //player = ((PlayerMovement)(GameObject.FindObjectOfType<PlayerMovement>())).gameObject.transform;
    }

    public void GetNewDestination()
    {
        int newTargetIndex = (int)Random.Range(0.0f, destinationVolumes.Length);
        if (newTargetIndex == currentTargetIndex)
        {
            if (currentTargetIndex == destinationVolumes.Length - 1)
                currentTargetIndex = 0;
            else
                currentTargetIndex = newTargetIndex + 1;
        }
        else
            currentTargetIndex = newTargetIndex;
        GetNewDestination(currentTargetIndex);
    }

    private void GetNewDestination(int index)
    {
        BoxCollider currentCollider = destinationVolumes[index];
        Vector3 dest = currentCollider.bounds.center
            + new Vector3(Random.Range(-currentCollider.bounds.size.x / 2, currentCollider.bounds.size.x / 2)
            , 0, Random.Range(-currentCollider.bounds.size.z / 2, currentCollider.bounds.size.z / 2));
        agent.destination = dest;
        newDestinationAssigned = true;

    }

    public void GetTargetAimlessly()
    {
        RaycastHit hitFwd, hitRight, hitLeft, hitDown, hitBack;
        bool lookBack = false;
        Physics.Raycast(flyRelativeMovement.transform.position, transform.right, out hitRight);
        Physics.Raycast(flyRelativeMovement.transform.position, -transform.right, out hitLeft);
        Physics.Raycast(flyRelativeMovement.transform.position, transform.forward, out hitFwd);
        Physics.Raycast(flyRelativeMovement.transform.position, transform.forward, out hitBack);
        RaycastHit hitMax = hitFwd;
        float distMax = hitFwd.distance;
        float r = Random.Range(0, 1.0f);
        if(r<0.3f)
        {
            hitMax = hitFwd;
            distMax = hitFwd.distance;
        }
        else if(r>=0.3f && r<0.6f)
        {
            hitMax = hitRight;
            distMax = hitRight.distance;
        }
        else if(r>=0.6f && r<0.9f)
        {
            hitMax = hitLeft;
            distMax = hitLeft.distance;
        }
        else
        {
            hitMax = hitBack;
            distMax = hitBack.distance;
        }
        if((player.position-transform.position).magnitude<playerAwarenessDistance)
        {
            RaycastHit avoidHit;
            Vector3 dir = (transform.position - player.position).normalized;
            if (Physics.Raycast(flyRelativeMovement.transform.position, dir, out avoidHit))
            {
                hitMax = avoidHit;
                distMax = avoidHit.distance;
            }
        }
        Vector3 xzCoord = transform.forward + transform.position;
        xzCoord = (hitMax.point - transform.position).normalized * Random.Range(distMax*0.65f, distMax-agent.radius);
        Physics.Raycast(xzCoord, Vector3.down, out hitDown);

        NavMeshHit nmh;
        if (NavMesh.SamplePosition(hitDown.point, out nmh, 5.0f, NavMesh.AllAreas))
        {
            agent.destination = nmh.position;
            newDestinationAssigned = true;
        }
        else
        {
            //if its not on the navmesh get a point normally
            GetNewDestination();
        }
    }

    void Update()
    {
        if(!canLand)
        {
            landCooldownTimer += Time.deltaTime;
            if(landCooldownTimer>=landCoolDown)
            {
                canLand = true;
                landCooldownTimer = 0.0f;
            }
        }
        if (agent.remainingDistance < agent.stoppingDistance && !newDestinationAssigned)
        {
            if (flyRelativeMovement.GetFlyMode() == FlyMode.TRAVELLING)
            {
                float landDraw = Random.Range(0.0f, 100.0f);
                if (landDraw <= landChance && canLand)
                {
                    flyRelativeMovement.SetFlyMode(FlyMode.FLOOR_LANDING);
                    canLand = false;
                }
                else
                    GetTargetAimlessly();
            }
        }
        else
            newDestinationAssigned = false;

    }
}
