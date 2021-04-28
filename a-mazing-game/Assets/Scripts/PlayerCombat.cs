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
    
    private float attackRate = 1f;
    private float nextAttack;

    public int score;
    
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
        // animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 1);
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
        
        // yield return new WaitForSeconds(1f);
        // animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
