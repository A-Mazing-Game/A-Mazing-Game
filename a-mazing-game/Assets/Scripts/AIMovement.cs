using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Transform attackPoint;
    public LayerMask playerLayers;
    public GameObject coins;

    public float lookRadius;
    public float wanderRadius;

    public float attackRange = 1f;
    public int attackDamage = 20;
    
    public int maxHealth = 100;
    public int currentHealth;
    
    private Animator animator;
    
    // private Vector3 wanderWaypoint;
 
    private float wanderSpeed = 1.25f;
    private float runSpeed = 2.25f;
    
    private float attackRate = 2.5f;
    private float nextAttack;

    void Start()
    {
         agent = GetComponent<NavMeshAgent>();
         animator = GetComponentInChildren<Animator>();
         currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        // Distance to the target
        float distance = Vector3.Distance(player.position, transform.position);
        
        // If not inside the lookRadius
        if (distance >= lookRadius)
        {
            Wander();
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance 
                                   && !agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                agent.ResetPath();
                NavMeshPath path = new NavMeshPath();
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.CalculatePath(newPos, path);
                if (agent.pathStatus != NavMeshPathStatus.PathPartial)
                {
                    agent.SetDestination(newPos);
                }
            }
        }
        
        if (distance < lookRadius)
        {
            FaceTarget();
            // If within attacking distance
            if (distance < agent.stoppingDistance)
            {
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
                Run();
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) 
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
     
        return navHit.position;
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
        int health = currentHealth;
        animator.SetTrigger("Swing");
        yield return new WaitForSeconds(0.9f);
        if (health == currentHealth)
        {
            Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers);

            // yield return new WaitForSeconds(0.5f);
            foreach (Collider player in hitPlayers)
            {
                player.GetComponent<PlayerCombat>().TakePlayerDamage(attackDamage);
                Debug.Log("Player hit!");
            }
        }
        
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // Rotate to face the target
    void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Play hurt animation
        nextAttack = Time.time + attackRate;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        
        // Play death animation
        animator.SetBool("IsDead", true);
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
        Instantiate(coins, agent.transform.position, Quaternion.identity);
    }
}
