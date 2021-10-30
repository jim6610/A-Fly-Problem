using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FlyNavigator : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject destinationsContainer;
    private BoxCollider[] destinationVolumes;
    private int currentTargetIndex;
    private Vector3 destination;
    private FlyRelativeMovmement flyRelativeMovement;
    private bool newDestinationAssigned;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        flyRelativeMovement = GetComponentInChildren<FlyRelativeMovmement>();
        destinationVolumes = destinationsContainer.GetComponentsInChildren<BoxCollider>();
        currentTargetIndex = 0;
        GetNewDestination(currentTargetIndex);
    }

    public void GetNewDestination()
    {
        int newTargetIndex = (int)Random.Range(0.0f, destinationVolumes.Length);
        if (newTargetIndex == currentTargetIndex)
        {
            print("new destination same as last destination");
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


    void Update()
    {
        if (!newDestinationAssigned)
        {
            if (flyRelativeMovement.GetFlyMode() == FlyMode.TRAVELLING)
                if (agent.remainingDistance < 0.1f)
                {
                    flyRelativeMovement.SetFlyMode(FlyMode.FLOOR_LANDING);
                }
        }
        else
            newDestinationAssigned = false;
    }
}
