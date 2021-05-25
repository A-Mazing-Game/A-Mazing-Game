using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    #region public members
    public Animator animator;
    public Transform attackPoint;
    public GameOverScreen GameOverScreen;
    public GameObject hud;
    public LayerMask enemyLayers;
    public TimeSpan elapsed;
    public float attackRange;
    public bool controlEnabled;
    public bool heavyAttack;
    public bool isDead;
    public float attackRate = 1f;
    // public InventoryItemBase currentItem;
    #endregion
    
    #region private members
    private DateTime startTime;
    private DateTime endTime;
    private FpsMovement fps;
    private PlayerStats playerStats;
    private FpsMovement movement;
    // private CharacterController cc;
    private float punchRate = 0.6f;
    private float nextPunch;
    private float nextAttack;
    private int attackDamage;
    private bool showingEnd;
    private bool canHook;
    private float attackChainCounter;
    #endregion

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        movement = GetComponent<FpsMovement>();
        // cc = GetComponent<CharacterController>();
        fps = GetComponent<FpsMovement>();
        // currentItem = GetComponent<PlayerController>().mCurrentItem;
        // attackDamage = playerStats.attackDamage;
        startTime = DateTime.Now;
        endTime = DateTime.Now;
        controlEnabled = true;
    }
    
    void Update()
    {
        if (!isDead && fps.IsArmed)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate;
                StartCoroutine(Attack());
            }
        }
        else if (!isDead && !fps.IsArmed)
        {
            if (!canHook && Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextPunch)
            {
                animator.speed = 1.5f;
                animator.SetTrigger("Punch");
                nextPunch = Time.time + punchRate;
                StartCoroutine(Punch());
            }
            
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Left Punch"))
            {
                canHook = true;
                attackChainCounter = Time.time;
            }
            else if (Time.time - attackChainCounter > 0.5f)
            {
                canHook = false;
            }
            
            if (canHook && Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextPunch)
            {
                animator.speed = 1.5f;
                animator.SetTrigger("Right Hook");
                nextPunch = Time.time + punchRate;
                StartCoroutine(Punch());
            }
        }
    }

    private IEnumerator Punch()
    {
        // int damage = 20;
        playerStats.attackDamage = 20;
        // Play attack animation
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Left Punch"))
        {
            yield return new WaitForSeconds(0.3f);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Right Hook"))
        {
            yield return new WaitForSeconds(0.5f);
        }
        // Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        // foreach (Collider enemy in hitEnemies)
        // {
        int length = hitEnemies.Length;
        if (length > 0)
        {
            StartCoroutine(hitEnemies[length - 1].GetComponent<AIMovement>().TakeDamage(playerStats.attackDamage));
            // Debug.Log(enemy.name + " hit!");
            if (hitEnemies[length - 1].GetComponent<AIMovement>().currentHealth <= 0)
            {
                playerStats.enemiesKilled++;
            }
        }
        // }
    }
    private IEnumerator Attack()
    {
        int attackType;
        float currentHealth = playerStats.currentHealth;
        float currentOvershield = playerStats.currentOvershield;
        // Play attack animation
        animator.SetTrigger("Attack");
        if (Input.GetKey(KeyCode.LeftShift) && movement.isSprintingForward)
        {
            if (fps.CarriesItem("Sword Epic"))
            {
                animator.speed = 1f;
                attackType = 3;
            }
            else
            {
                animator.speed = 0.75f;
                attackType = 1;
            }
            animator.SetFloat("AttackMode", 0.5f);
        }
        else
        {
            if (fps.CarriesItem("Sword Epic"))
            {
                attackType = 2;
            }
            else
            {
                attackType = 0;
            }
            animator.SetFloat("AttackMode", 0f);
        }
        
        if (attackType == 0)
        {
            // Katana normal attack
            playerStats.attackDamage = 40;
            yield return new WaitForSeconds(0.1f);
            controlEnabled = true;
        }
        else if (attackType == 1)
        {
            // Katana special attack
            playerStats.attackDamage = 80;
            heavyAttack = true;
            movement.runSpeed = 4.0f;
            yield return new WaitForSeconds(0.9f);
            controlEnabled = false;
        }
        else if (attackType == 2)
        {
            // Great sword normal attack
            playerStats.attackDamage = 60;
            yield return new WaitForSeconds(0.7f);
            controlEnabled = true;
        }
        else if (attackType == 3)
        {
            // Great sword special attack
            playerStats.attackDamage = 100;
            heavyAttack = true;
            movement.runSpeed = 4.0f;
            yield return new WaitForSeconds(1.4f);
            controlEnabled = false;
        }

        if (currentHealth == playerStats.currentHealth && currentOvershield == playerStats.currentOvershield)
        {
            // Detect enemies in range of attack
            Collider[] hitEnemies =
                Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers); // Damage them
            
            foreach (Collider enemy in hitEnemies)
            {
                StartCoroutine(enemy.GetComponent<AIMovement>().TakeDamage(playerStats.attackDamage));
                // Debug.Log(enemy.name + " hit!");
                if (enemy.GetComponent<AIMovement>().currentHealth <= 0)
                {
                    playerStats.enemiesKilled++;
                }
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
    }
}
