using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class SpiderNavigator : MonoBehaviour
{
    [SerializeField]
    private float wallAwarenessDistance;
    [Range(1.0f, 99.0f)]
    public float stopChance;
    [SerializeField]
    private float stopCoolDown;
    [SerializeField]
    private float sightDistance;
    [SerializeField]
    private float playerAwarenessDistance;
    [SerializeField]
    private float playerSneakAwarenessDistance;
    [SerializeField]
    private float playerSprintAwarenenessDistance;
    [SerializeField]
    private float minimumStoppingTime = 1.0f;
    [SerializeField]
    private float maximumStoppingTime = 3.0f;

    private NavMeshAgent agent;
    public BoxCollider safeDestination = null;
    private float stoppingTime;
    private Animator animator;

    public GameObject player;

    float stopCooldownTimer = 0.0f;
    bool canStop = false;
    bool isStopped = false;
    bool isDead = false;
    private Vector3 safePosition;

    void Start()
    {
        // FindObjectOfType<AudioManager>().Play("Fly");

        Random.InitState(System.DateTime.Now.Millisecond);
        agent = GetComponent<NavMeshAgent>();
        GetTargetAimlessly();
        agent.stoppingDistance = agent.radius;
        animator = GetComponentInChildren<Animator>();
        isDead = false;
        safePosition = transform.position;
    }


    private void GetSafeDestination()
    {
        if (safeDestination)
        {
            BoxCollider currentCollider = safeDestination;
            Vector3 dest = currentCollider.bounds.center
                + new Vector3(Random.Range(-currentCollider.bounds.size.x / 2, currentCollider.bounds.size.x / 2)
                , 0, Random.Range(-currentCollider.bounds.size.z / 2, currentCollider.bounds.size.z / 2));
            agent.destination = dest;
        }
        else
            agent.destination = safePosition;
    }

    public void GetTargetAimlessly()
    {
        RaycastHit hitFwd, hitRight, hitLeft, hitDown, hitBack, hitFwdRight, hitFwdLeft;
        bool didHitFwd = false, didHitRight = false, didHitLeft = false, didHitFwdRight = false, didHitFwdLeft = false, didHitBack = false;
        didHitRight = Physics.Raycast(transform.position, transform.right, out hitRight, sightDistance);
        didHitLeft = Physics.Raycast(transform.position, -transform.right, out hitLeft, sightDistance);
        didHitFwd = Physics.Raycast(transform.position, transform.forward, out hitFwd, sightDistance);
        didHitFwdLeft = Physics.Raycast(transform.position, transform.forward - transform.right, out hitFwdLeft, sightDistance);
        didHitFwdRight = Physics.Raycast(transform.position, transform.forward + transform.right, out hitFwdRight, sightDistance);
        didHitBack = Physics.Raycast(transform.position, -transform.forward, out hitBack, sightDistance);

        RaycastHit hitMax = hitFwd;
        float distMax = hitFwd.distance;
        bool didHit = didHitFwd;
        Vector3 dir = transform.forward;
        Vector3 xzCoord = transform.forward + transform.position;
        //If none of the fwd directions hit then use them
        if (!didHitFwd || !didHitFwdLeft || !didHitFwdRight)
        {
            //print("Trying forward directions");
            distMax = sightDistance;
            bool set = false;
            didHit = false;
            if (!didHitFwd)
            {
                set = true;
                dir = transform.forward;
            }
            if (!didHitFwdRight)
            {
                if (!set)
                    dir = (transform.forward + transform.right).normalized;
                else
                {
                    float r = Random.Range(0, 1.0f);
                    if (r > 0.8f)
                        dir = (transform.forward + transform.right).normalized;
                    set = true;
                }
            }
            if (!didHitFwdLeft)
            {
                if (!set)
                    dir = (transform.forward - transform.right).normalized;
                else
                {
                    float r = Random.Range(0, 1.0f);
                    if (r > 0.8f)
                        dir = (transform.forward - transform.right).normalized;
                    set = true;
                }
            }

        }
        else
        {
            //print("trying other directions");
            //otherwise randomly choose between the back/sides
            float r = Random.Range(0, 1.0f);
            if (r < 0.333f)
            {
                hitMax = hitRight;
                distMax = hitRight.distance;
                didHit = didHitRight;
                dir = transform.right;
            }
            else if (r >= 0.333f && r < 0.666f)
            {
                hitMax = hitLeft;
                distMax = hitLeft.distance;
                didHit = didHitLeft;
                dir = -transform.right;
            }
            else
            {
                hitMax = hitBack;
                distMax = hitBack.distance;
                didHit = didHitBack;
                dir = -transform.forward;
            }
        }

        float avoidPlayer = Random.Range(0, 1);
        //If the players withtin the awareness distance we want to override this behavior
        if (PlayerInRange(transform) && avoidPlayer>0.5f)
        {
            RaycastHit avoidHit;
            Vector3 playerDir = (transform.position - player.transform.position).normalized;

            didHit = false;
            if (Physics.Raycast(transform.position, new Vector3(playerDir.x, 0, playerDir.z), out avoidHit, sightDistance))
            {
                //would run into the wall so dont go this direction...
                
            }
            else
            {
                dir = playerDir;
            }
        }

        if (didHit)
        {
            xzCoord = transform.position + (hitMax.point - transform.position).normalized * Random.Range(distMax * 0.5f, distMax - agent.radius);
        }
        else
        {
            xzCoord = transform.position + dir * sightDistance;
        }

        canStop = false;
        NavMeshHit nmh;
        if (NavMesh.SamplePosition(xzCoord, out nmh, 1.0f, NavMesh.AllAreas))
        {
            agent.destination = nmh.position;
        }
        else
        {
            //if its not on the navmesh get a point normally
            GetSafeDestination();
        }
    }

    void Update()
    {
        if (!isDead)
        {
            if (!canStop)
            {
                stopCooldownTimer += Time.deltaTime;
                if (stopCooldownTimer >= stopCoolDown)
                {
                    canStop = true;
                    stopCooldownTimer = 0.0f;
                }
            }

            if (agent.remainingDistance < agent.stoppingDistance && !isStopped)
            {
                float landDraw = Random.Range(0.0f, 100.0f);
                if (landDraw <= stopChance && canStop)
                {
                    isStopped = true;
                    stoppingTime = Random.Range(minimumStoppingTime, maximumStoppingTime);
                }
                else
                    GetTargetAimlessly();
            }
            else if (isStopped)
            {
                Stop();
            }

            animator.SetBool("animIsWalking", !isStopped);
            animator.SetBool("animIsEating", isStopped);
        }
        else
            Dying();

    }

    private void Dying()
    {
        animator.SetBool("animIsWalking", false);
        animator.SetBool("animIsEating", false);
        animator.SetBool("animIsDead", true);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void SetDead()
    {
        isDead = true;
    }


    private void Stop()
    {

        stopCooldownTimer += Time.deltaTime;
        if(stopCooldownTimer>stoppingTime || PlayerInRange(transform))
        {
            isStopped = false;
            stopCooldownTimer = 0.0f;
            GetTargetAimlessly();
        }
    }    
    

    public bool PlayerInRange(Transform otherTransform)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        var detectionDistance = playerMovement.IsSprinting ? playerSprintAwarenenessDistance :
            playerMovement.IsCrouching ? playerSneakAwarenessDistance : playerAwarenessDistance;

        return (player.gameObject.transform.position - otherTransform.position).magnitude < detectionDistance;
    }
}
