using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    
    public float attackRange;
    public int attackDamage = 40;
    public int maxHealth = 100;
    private int currentHealth;
    
    private float attackRate = 1f;
    private float nextAttack;

    public int score;

    void Start()
    {
        currentHealth = maxHealth;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextAttack)
        {
            nextAttack = Time.time + attackRate;
            Attack();
        }
    }
    
    private void Attack()
    {
        // Play attack animation
        animator.speed = 1;
        animator.SetTrigger("Attack");
        
        // Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        
        // Damage them
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.GetComponent<AIMovement>().TakeDamage(attackDamage))
            {
                score++;
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
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        Debug.Log("Player died!");
        animator.SetBool("IsDead", true);
        GetComponent<Collider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<FpsMovement>().enabled = false;
        // this.enabled = false;
    }
}
