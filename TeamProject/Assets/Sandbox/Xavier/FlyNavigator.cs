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

    private void MoveAimlesssly()
    {

    }

    void Update()
    {
        switch(mode)
        {
            //case AgentMode.TARGET:
            //    aimlessTargetTimer += Time.deltaTime;
            //    if(aimlessTargetTimer>aimlessTargetCooldown)
            //    {
            //        aimlessTargetTimer = 0.0f;
            //        mode = AgentMode.AIMLESS;
            //        return;
            //    }
            //    if (!newDestinationAssigned)
            //    {
            //        if (flyRelativeMovement.GetFlyMode() == FlyMode.TRAVELLING)
            //            if (agent.remainingDistance < 0.1f)
            //            {
            //                flyRelativeMovement.SetFlyMode(FlyMode.FLOOR_LANDING);
            //            }
            //    }
            //    else
            //        newDestinationAssigned = false;
            //    break;
            case AgentMode.AIMLESS:
                if (!aimlessTargetSet)
                {
                    RaycastHit hit, hitRight, hitLeft;
                    Physics.Raycast(transform.position, transform.right, out hitRight);
                    Physics.Raycast(transform.position, -transform.right, out hitLeft);
                    Physics.Raycast(transform.position, transform.forward, out hit);
                    if(hit.distance>hitRight.distance && hit.distance>hitLeft.distance)
                    {
                        agent.destination = hit.point - transform.forward * wallAwarenessDistance;
                        aimlessTargetSet = true;
                    }
                    else if(hitLeft.distance>hitRight.distance)
                    {
                        agent.destination = hitLeft.point + transform.right * wallAwarenessDistance;
                        aimlessTargetSet = true;
                    }
                    else
                    {
                        agent.destination = hitRight.point - transform.right * wallAwarenessDistance;
                        aimlessTargetSet = true;
                    }
                       // mode = AgentMode.TARGET;
                }
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    //agent.destination = targetDestination;
                    //mode = AgentMode.TARGET;
                    aimlessTargetSet = false;
                }
                break;
        }

    }
}
