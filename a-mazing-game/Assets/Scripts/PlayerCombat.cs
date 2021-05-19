using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float attackRange;
    public bool controlEnabled;
    public bool heavyAttack;
    public bool isDead;
    // public InventoryItemBase currentItem;
    
    private float attackRate = 1f;
    private float nextAttack;
    private int attackDamage;
    private bool showingEnd;
    private DateTime startTime;
    private DateTime endTime;
    public TimeSpan elapsed;
    public GameOverScreen GameOverScreen;
    public GameObject hud;

    private PlayerStats playerStats;
    private FpsMovement movement;
    private CharacterController cc;
    
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        movement = GetComponent<FpsMovement>();
        cc = GetComponent<CharacterController>();
        // currentItem = GetComponent<PlayerController>().mCurrentItem;
        // attackDamage = playerStats.attackDamage;
        startTime = DateTime.Now;
        endTime = DateTime.Now;
        controlEnabled = true;
    }
    
    void Update()
    {
        if (!isDead) // && mIsControlEnabled)
        {
            // Interact with the item
            // if (mInteractItem != null && Input.GetKeyDown(KeyCode.F))
            // {
            //     // Interact animation
            //     mInteractItem.OnInteractAnimation(_animator);
            // }

            // Execute action with item
            // if (currentItem != null && Input.GetMouseButtonDown(0))
            // {
            if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate;
                StartCoroutine(Attack());
            }
                // // Dont execute click if mouse pointer is over uGUI element
                // if (!EventSystem.current.IsPointerOverGameObject())
                // {
                //     // TODO: Logic which action to execute has to come from the particular item
                //     _animator.SetTrigger("attack_1");
                // }
            // }
        }
    }
    
    private IEnumerator Attack()
    {
        float attackType;
        int damage = playerStats.attackDamage;
        // Play attack animation
        animator.SetTrigger("Attack");
        if (Input.GetKey(KeyCode.LeftShift) && movement.isSprintingForward)
        {
            attackType = 0.5f;
            animator.speed = 0.5f;
            animator.SetFloat("AttackMode", 0.5f);
        }
        else
        {
            attackType = 0f;
            animator.SetFloat("AttackMode", 0);
        }

        // Detect enemies in range of attack
        if (attackType == 0f)
        {
            damage = playerStats.attackDamage;
            yield return new WaitForSeconds(0.1f);
            controlEnabled = true;
        }
        else if (attackType == 0.5f)
        {
            damage = 100;
            heavyAttack = true;
            movement.runSpeed = 1.5f;
            yield return new WaitForSeconds(0.9f);
            controlEnabled = false;
        }
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider enemy in hitEnemies)
        {
            StartCoroutine(enemy.GetComponent<AIMovement>().TakeDamage(damage));
            Debug.Log(enemy.name + " hit!");
            if (enemy.GetComponent<AIMovement>().currentHealth <= 0)
            {
                playerStats.enemiesKilled++;
            }
        }
        // cc.enabled = false;
        yield return new WaitForSeconds(0.5f);
        heavyAttack = false;
        movement.runSpeed = 4f;
        // cc.enabled = true;
        controlEnabled = true;
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
        isDead = true;
        endTime = DateTime.Now;
        elapsed = endTime - startTime;
        hud.SetActive(false);
        GameOverScreen.Setup();
        // this.enabled = false;
    }
}
