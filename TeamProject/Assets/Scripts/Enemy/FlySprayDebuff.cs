using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlySprayDebuff : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]
    private float debuffTime = 3.0f;
    [SerializeField]
    private float slowdown = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    public void Sprayed()
    {
        //Todo: Status Effects

        agent.speed = agent.speed / slowdown;
        agent.acceleration = agent.acceleration / slowdown;
        agent.angularSpeed = agent.angularSpeed / slowdown;
        StartCoroutine(SprayedCoroutine());
    }
    IEnumerator SprayedCoroutine()
    {
        yield return new WaitForSeconds(debuffTime);
        agent.speed = agent.speed * slowdown;
        agent.acceleration = agent.acceleration * slowdown;
        agent.angularSpeed = agent.angularSpeed * slowdown;
    }
}
