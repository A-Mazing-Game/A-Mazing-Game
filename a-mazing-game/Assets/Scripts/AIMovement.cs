using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    public Transform player; 
    public NavMeshAgent agent;
    
    public float lookRadius;
    public float wanderDistance;
 
    
    private Animator animator;
    
    private Vector3 wanderWaypoint;
 
    private float wanderSpeed = 1.5f;
    private float runSpeed = 3f;
    
    private float attackRate = 3f;
    private float nextAttack;
    private float stopTime;

    void Start()
    {
         agent = GetComponent<NavMeshAgent>();
         animator = GetComponentInChildren<Animator>();
         // combat = GetComponent<CharacterCombat>();
    }

    void FixedUpdate()
    {
        // Distance to the target
        float distance = Vector3.Distance(player.position, transform.position);
        
        //if not inside the lookRadius
        if (distance >= lookRadius)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        Wander();
                        wanderWaypoint = new Vector3(transform.position.x + Random.Range(-wanderDistance, wanderDistance), 0,
                            transform.position.z + Random.Range(-wanderDistance, wanderDistance));
                        agent.SetDestination(wanderWaypoint);
                    }
                }
            }
        }
        
        if (distance < lookRadius)
        {
            // If within attacking distance
            if (distance < agent.stoppingDistance)
            {
                // CharacterStats targetStats = target.GetComponent<CharacterStats>();
                // if (targetStats != null)
                // {
                // combat.Attack(targetStats);
                // }
                Idle();
                if (Time.time > nextAttack)
                {
                    nextAttack = Time.time + attackRate;
                    StartCoroutine(Slash());
                }
            }
            else
            {
                // Move towards the target
                agent.SetDestination(player.position);
                FaceTarget(); 
                Run();
            }
        }
    }

    private void Idle()
    {
        animator.SetFloat("Speed", 0f, 0.2f, Time.deltaTime);
    }

    private void Wander()
    {
        agent.speed = wanderSpeed;
        animator.SetFloat("Speed", 0.5f, 0.2f, Time.deltaTime);
    }
    
    private void Run()
    {
        agent.speed = runSpeed;
        animator.SetFloat("Speed", 1f, 0.2f, Time.deltaTime);
    }

    private IEnumerator Slash()
    {
        agent.isStopped = true;
        animator.SetTrigger("Swing");
        yield return new WaitForSeconds(2f);
        agent.isStopped = false;
    }

    // Rotate to face the target
    void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
