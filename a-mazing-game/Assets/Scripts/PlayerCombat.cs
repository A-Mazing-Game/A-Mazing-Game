using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float attackRange;

    private float attackRate = 1f;
    private float nextAttack;
    private int attackDamage;
    private bool showingEnd;
    private DateTime startTime;
    private DateTime endTime;
    public TimeSpan elapsed;
    public GameOverScreen GameOverScreen;

    private PlayerStats playerStats;
    
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        // attackDamage = playerStats.attackDamage;
        startTime = DateTime.Now;
        endTime = DateTime.Now;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextAttack)
        {
            nextAttack = Time.time + attackRate;
            StartCoroutine(Attack());
        }
    }
    
    private IEnumerator Attack()
    {
        // Play attack animation
        animator.SetTrigger("Attack");
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.speed = 0.5f;
            animator.SetFloat("AttackMode", 0.5f);
        }
        else
            animator.SetFloat("AttackMode", 0);

        // Detect enemies in range of attack
        yield return new WaitForSeconds(0.1f);
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        
        // Damage them
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<AIMovement>().TakeDamage(playerStats.attackDamage);
            Debug.Log(enemy.name + " hit!");
            if (enemy.GetComponent<AIMovement>().currentHealth <= 0)
            {
                playerStats.enemiesKilled++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void TakePlayerDamage(int damage)
    {
        int currentOvershield = playerStats.currentOvershield;
        int currentHealth = playerStats.currentHealth;
        int remainder;
        if (currentOvershield > 0)
        {
            playerStats.SubtractOvershield(damage);
            remainder = damage - currentOvershield;
            if (remainder > 0)
                currentHealth = playerStats.SubtractHealth(remainder);
        }
        else
        {
            playerStats.currentOvershield = 0;
            currentHealth = playerStats.SubtractHealth(damage);
        }

        animator.SetTrigger("Hurt");
        
        if (currentHealth <= 0)
        {
            PlayerDie();
        }
    }

    public void endGame(GameObject trigger, GameObject other)
    {
        /*
         * This method will end the game when the end of the
         * maze object is touched
         */

        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            PlayerDie();
        }

    }

    private void PlayerDie()
    {
        // Debug.Log("Player died!");
        // animator.SetBool("IsDead", true);
        // GetComponent<Collider>().enabled = false;
        // GetComponent<CharacterController>().enabled = false;
        // GetComponent<FpsMovement>().enabled = false;
        //TimeSpan time = GetComponent<GameController>().elapsed;
        endTime = DateTime.Now;
        elapsed = endTime - startTime;
        GameOverScreen.Setup();
        // this.enabled = false;
    }
}
