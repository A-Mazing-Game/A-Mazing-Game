using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    public float lookRadius;
    // public float wakeRadius;
    public float wanderDistance;
    private Vector3 wanderWaypoint;

    public Transform player; 
    public NavMeshAgent agent;
    // CharacterCombat combat;
 
    private Animator animator;
 
    private float wanderSpeed = 1.5f;
    private float runSpeed = 3f;
    
    private float attackRate = 2.5f;
    private float nextAttack;
    private float stopTime;
    private float attackDelay = 0.5f;


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
            // Move towards the target
            agent.SetDestination(player.position);
            FaceTarget(); 
            Run();

            // If within attacking distance
            if (distance < agent.stoppingDistance)
            {
                // CharacterStats targetStats = target.GetComponent<CharacterStats>();
                // if (targetStats != null)
                // {
                // combat.Attack(targetStats);
                // }
                // Idle();
                if (Time.time > nextAttack)
                {
                    nextAttack = Time.time + attackRate;
                    Slash();
                }
            }
        }
    }

    private void Idle()
    {
        agent.speed = 0;
        animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }

    private void Wander()
    {
        agent.speed = wanderSpeed;
        animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }
    
    private void Run()
    {
        agent.speed = runSpeed;
        animator.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
    }

    private void Slash()
    {
        animator.SetTrigger("Swing");
    }

    // Rotate to face the target
    void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
