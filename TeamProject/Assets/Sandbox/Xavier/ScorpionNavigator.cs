using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class ScorpionNavigator : MonoBehaviour
{
    [SerializeField]
    private float sightDistance;
    [SerializeField]
    private float playerAwarenessDistance;
    [SerializeField]
    private float playerSneakAwarenessDistance;
    [SerializeField]
    private float playerSprintAwarenenessDistance;
    [SerializeField]
    private float attackCoolDownTime = 5.0f;

    private NavMeshAgent agent;
    public BoxCollider safeDestination = null;
    private Animator animator;

    public GameObject player;

    float attackCooldownTimer = 0.0f;
    bool canAttack = true;
    bool isDead = false;
    private Vector3 safePosition;
    private float attackAnimDist = 5.0f;
    private Vector3 previousDest;

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        player = (GameObject.FindObjectOfType<PlayerMovement>()).gameObject;
        agent = GetComponent<NavMeshAgent>();
        GetTargetAimlessly(false);
        agent.stoppingDistance = agent.radius;
        animator = GetComponentInChildren<Animator>();
        isDead = false;
        safePosition = transform.position;
        previousDest = safePosition;
    }



    public void GetTargetAimlessly(bool avoidPlayer)
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

        if (avoidPlayer)
        {
            //If the players withtin the awareness distance we want to override this behavior
            if (PlayerInRange(transform))
            {
                RaycastHit avoidHit;
                Vector3 playerDir = (transform.position - player.transform.position).normalized;


                if (Physics.Raycast(transform.position, new Vector3(playerDir.x, 0, playerDir.z), out avoidHit, sightDistance))
                {
                    //would run into the wall so dont go this direction...
                }
                else
                {
                    didHit = false;
                    dir = playerDir;
                }
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

        NavMeshHit nmh;
        if (NavMesh.SamplePosition(xzCoord, out nmh, 1.0f, NavMesh.AllAreas))
        {
            //need to check if the same spot is picked
            if ((nmh.position - agent.destination).magnitude > 1.0f)
                agent.destination = nmh.position;
            else
                agent.destination = safePosition;
        }
        else
        {
            //if its not on the navmesh get a point normally
            agent.destination = safePosition;
        }
    }

    void Update()
    {
        if (!isDead)
        {
            animator.SetBool("animIsWalking", true);
            animator.SetBool("animIsAttacking", false);
            animator.SetBool("animIsStinging", false);

            if (canAttack)
            {
                if (PlayerInRange(transform))
                    agent.destination = player.transform.position;
                else if (agent.remainingDistance < agent.stoppingDistance)
                    GetTargetAimlessly(false);
            }
            else
            {
                attackCooldownTimer += Time.deltaTime;
                if(attackCooldownTimer >= attackCoolDownTime)
                {
                    canAttack = true;
                    attackCooldownTimer = 0.0f;
                }
                if (agent.remainingDistance < agent.stoppingDistance)
                    GetTargetAimlessly(true);
            }

        }
        else
            Dying();

    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject == player.gameObject && canAttack)
        {
            animator.SetBool("animIsStinging", true);
            canAttack = false;
            GetTargetAimlessly(true);
        }
    }

    private void Dying()
    {
        animator.SetBool("animIsWalking", false);
        animator.SetBool("animIsAttacking", false);
        animator.SetBool("animIsStinging", false);
        animator.SetBool("animIsDying", true);
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




    public bool PlayerInRange(Transform otherTransform)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        var detectionDistance = playerMovement.IsSprinting ? playerSprintAwarenenessDistance :
            playerMovement.IsCrouching ? playerSneakAwarenessDistance : playerAwarenessDistance;

        return (player.gameObject.transform.position - otherTransform.position).magnitude < detectionDistance;
    }
}
