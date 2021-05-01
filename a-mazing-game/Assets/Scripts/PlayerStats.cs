using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int attackDamage = 40;
    public int maxHealth;
    public int currentHealth;
    public int enemiesKilled = 0;

    public HealthBar healthBar;
    public GameObject controller;

    private void Awake()
    {
        controller = GameObject.Find("Controller");
        maxHealth = controller.GetComponent<BaseStats>().maxHealth;
        currentHealth = controller.GetComponent<BaseStats>().maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
    }

    // Start is called before the first frame update

    public void SetUp()
    {
        maxHealth = controller.GetComponent<BaseStats>().maxHealth;
        currentHealth = controller.GetComponent<BaseStats>().maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
    }


    public void SubtractHealth(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void AddHealth(int health)
    {
        currentHealth -= health;
        if (currentHealth > maxHealth)
        {
            healthBar.SetHealth(maxHealth);
        }
        else
        {
            healthBar.SetHealth(currentHealth);

        }

    }

    public void IncreaseMaxHealth(int health)
    {
        int newHealth = maxHealth + health;
        healthBar.SetMaxHealth(newHealth);
        maxHealth = newHealth;
    }

}
