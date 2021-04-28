using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 2.0f;
    public LayerMask enemyLayers;
    
    private float attackRate = 1.5f;
    private float nextAttack;
    
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
        animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 1);
        animator.SetTrigger("Slash");
        yield return new WaitForSeconds(1.5f);
        animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 0);
        
        // Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        
        // Damage them
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Enemy hit" + enemy.name);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
