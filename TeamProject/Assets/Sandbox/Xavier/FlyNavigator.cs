using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum AgentMode
{
    TARGET,
    AIMLESS
}

[RequireComponent(typeof(NavMeshAgent))]
public class FlyNavigator : MonoBehaviour
{
    [SerializeField]
    private float wallAwarenessDistance;
    [Range(1.0f, 99.0f)]
    public float landChance;

    private NavMeshAgent agent;
    public GameObject destinationsContainer;
    private BoxCollider[] destinationVolumes;
    private int currentTargetIndex;
    private Vector3 destination;
    private FlyRelativeMovmement flyRelativeMovement;
    private bool newDestinationAssigned;
    private AgentMode mode;
    private bool aimlessTargetSet;
    private Vector3 targetDestination;

    float aimlessTargetTimer = 0.0f;
    float aimlessTargetCooldown = 5.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        flyRelativeMovement = GetComponentInChildren<FlyRelativeMovmement>();
        destinationVolumes = destinationsContainer.GetComponentsInChildren<BoxCollider>();
        currentTargetIndex = 0;
        GetNewDestination(currentTargetIndex);
        mode = AgentMode.AIMLESS;
        aimlessTargetSet = false;
        agent.stoppingDistance = 3.0f;
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
        targetDestination = dest;
        agent.destination = dest;
        newDestinationAssigned = true;

    }

    public void GetTargetAimlessly()
    {
        RaycastHit hit, hitRight, hitLeft, hitDown, hitBack;
        bool lookBack = false;
        Physics.Raycast(flyRelativeMovement.transform.position, transform.right, out hitRight);
        Physics.Raycast(flyRelativeMovement.transform.position, -transform.right, out hitLeft);
        Physics.Raycast(flyRelativeMovement.transform.position, transform.forward, out hit);
        Physics.Raycast(flyRelativeMovement.transform.position, transform.forward, out hitBack);
        RaycastHit hitMax = hit;
        float distMax = hit.distance;
        if(hitRight.distance> distMax)
        {
            hitMax = hitRight;
            distMax = hitRight.distance;
        }
        if (hitLeft.distance > distMax)
        {
            hitMax = hitLeft;
            distMax = hitLeft.distance;
        }
        if (hitLeft.distance < wallAwarenessDistance && hitRight.distance < wallAwarenessDistance && hit.distance < wallAwarenessDistance)
        {
            if (hitBack.distance > distMax)
            {
                hitMax = hitBack;
                distMax = hitBack.distance;
            }
        }
        Vector3 xzCoord = transform.forward + transform.position;
        xzCoord = (hitMax.point - transform.position).normalized * Random.Range(1.0f, distMax);
        Physics.Raycast(xzCoord, Vector3.down, out hitDown);
        agent.destination = hitDown.point;
        newDestinationAssigned = true;
    }

    void Update()
    {

        if (agent.remainingDistance < agent.stoppingDistance && !newDestinationAssigned)
        {
            if (flyRelativeMovement.GetFlyMode() == FlyMode.TRAVELLING)
            {
                float landDraw = Random.Range(0.0f, 100.0f);
                if (landDraw <= landChance)
                {
                    flyRelativeMovement.SetFlyMode(FlyMode.FLOOR_LANDING);
                }
                else
                    GetTargetAimlessly();
            }
        }
        else
            newDestinationAssigned = false;

    }
}
