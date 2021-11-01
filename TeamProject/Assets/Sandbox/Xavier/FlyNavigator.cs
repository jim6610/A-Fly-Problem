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
    private float sightDistance;
    [SerializeField]
    private float playerAwarenessDistance;
    [SerializeField]
    private float playerSneakAwarenessDistance;
    [SerializeField]
    private float playerSprintAwarenenessDistance;

    private NavMeshAgent agent;
    public GameObject destinationsContainer;
    private BoxCollider[] destinationVolumes;
    private int currentTargetIndex;
    private FlyRelativeMovmement flyRelativeMovement;
    private bool newDestinationAssigned;

    public GameObject player;

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
        agent.stoppingDistance = 1.0f;
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
        RaycastHit hitFwd, hitRight, hitLeft, hitDown, hitBack, hitFwdRight, hitFwdLeft;
        bool didHitFwd = false, didHitRight = false, didHitLeft = false, didHitFwdRight = false, didHitFwdLeft = false, didHitBack = false;
        didHitRight = Physics.Raycast(flyRelativeMovement.transform.position, flyRelativeMovement.transform.right, out hitRight, sightDistance);
        didHitLeft = Physics.Raycast(flyRelativeMovement.transform.position, -flyRelativeMovement.transform.right, out hitLeft, sightDistance);
        didHitFwd = Physics.Raycast(flyRelativeMovement.transform.position, flyRelativeMovement.transform.forward, out hitFwd, sightDistance);
        didHitFwdLeft = Physics.Raycast(flyRelativeMovement.transform.position, flyRelativeMovement.transform.forward-transform.right, out hitFwdLeft, sightDistance);
        didHitFwdRight = Physics.Raycast(flyRelativeMovement.transform.position, flyRelativeMovement.transform.forward + transform.right, out hitFwdRight, sightDistance);
        didHitBack = Physics.Raycast(flyRelativeMovement.transform.position, -flyRelativeMovement.transform.forward, out hitBack, sightDistance);
        
        RaycastHit hitMax = hitFwd;
        float distMax = hitFwd.distance;
        bool didHit = didHitFwd;
        Vector3 dir = flyRelativeMovement.transform.forward;
        //print(transform.forward);
        Vector3 xzCoord = flyRelativeMovement.transform.forward + flyRelativeMovement.transform.position;
        //If none of the fwd directions hit then use them
        if (!didHitFwd || !didHitFwdLeft || !didHitFwdRight)
        {
            //print("Trying forward directions");
            distMax = sightDistance;
            bool set = false;
            didHit = false;
            if(!didHitFwd)
            {
                set = true;
                dir = flyRelativeMovement.transform.forward;
            }
            if(!didHitFwdRight)
            {
                if(!set)
                    dir = (flyRelativeMovement.transform.forward + flyRelativeMovement.transform.right).normalized;
                else
                {
                    float r = Random.Range(0, 1.0f);
                    if(r>0.8f)
                        dir = (flyRelativeMovement.transform.forward + flyRelativeMovement.transform.right).normalized;
                    set = true;
                }
            }
            if(!didHitFwdLeft)
            {
                if (!set)
                    dir = (flyRelativeMovement.transform.forward - flyRelativeMovement.transform.right).normalized;
                else
                {
                    float r = Random.Range(0, 1.0f);
                    if (r > 0.8f)
                        dir = (flyRelativeMovement.transform.forward - flyRelativeMovement.transform.right).normalized;
                    set = true;
                }
            }

        }
        else
        {
            //print("trying other directions");
            //otherwise randomly choose between the back/sides
            float r = Random.Range(0, 1.0f);
            if (r <0.333f)
            {
                hitMax = hitRight;
                distMax = hitRight.distance;
                didHit = didHitRight;
                dir = flyRelativeMovement.transform.right;
            }
            else if (r >= 0.333f && r < 0.666f)
            {
                hitMax = hitLeft;
                distMax = hitLeft.distance;
                didHit = didHitLeft;
                dir = -flyRelativeMovement.transform.right;
            }
            else
            {
                hitMax = hitBack;
                distMax = hitBack.distance;
                didHit = didHitBack;
                dir = -flyRelativeMovement.transform.forward;
            }
        }

        //If the players withtin the awareness distance we want to override this behavior
        if (PlayerInRange(transform))
        {
            RaycastHit avoidHit;
            Vector3 playerDir = (transform.position - player.transform.position).normalized;
            
            didHit = false;
            if (Physics.Raycast(flyRelativeMovement.transform.position, new Vector3(playerDir.x, 0, playerDir.z), out avoidHit, sightDistance))
            {
                hitMax = avoidHit;
                distMax = avoidHit.distance;
                didHit = true;
            }
        }

        if (didHit)
        {
            xzCoord = (hitMax.point - flyRelativeMovement.transform.position).normalized * Random.Range(distMax * 0.5f, distMax - agent.radius);
            //print("did hit");
        }
        else
        {
            //print("did hit");
            xzCoord = flyRelativeMovement.transform.position + dir * sightDistance;
        }
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
    
    public bool PlayerInRange(Transform otherTransform)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        
        var detectionDistance = playerMovement.IsSprinting ? playerSprintAwarenenessDistance : 
            playerMovement.IsCrouching ? playerSneakAwarenessDistance : playerAwarenessDistance;
        
        return (player.gameObject.transform.position - otherTransform.position).magnitude < detectionDistance;
    }
}
