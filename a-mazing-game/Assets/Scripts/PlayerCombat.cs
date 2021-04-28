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
    public int attackDamage = 40;
    public int maxHealth = 100;
    public int currentHealth;
    
    private float attackRate = 1f;
    private float nextAttack;
    public HealthBar healthBar;
    private bool showingEnd;
    private DateTime startTime;
    private DateTime endTime;
    public TimeSpan elapsed;
    public GameOverScreen GameOverScreen;
    //private GameController controller;

    public int score;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        //controller = GetComponent<GameController>();
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
        animator.speed = 1;
        animator.SetTrigger("Attack");
        
        // Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        
        // Damage them
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.GetComponent<AIMovement>().TakeDamage(attackDamage))
            {
                yield return new WaitForSeconds(1.0f);
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
        healthBar.SetHealth(currentHealth);
        print(healthBar.slider.value);

        if (currentHealth <= 0)
        {
            PlayerDie();
            //controller.endGame(null, null);
        }
    }

    private void PlayerDie()
    {
        Debug.Log("Player died!");
        animator.SetBool("IsDead", true);
        GetComponent<Collider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<FpsMovement>().enabled = false;
        //TimeSpan time = GetComponent<GameController>().elapsed;
        endTime = DateTime.Now;
        elapsed = endTime - startTime;
        GameOverScreen.Setup(score, elapsed);
        // this.enabled = false;
    }
}
