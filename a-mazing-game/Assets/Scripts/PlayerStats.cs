using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int attackDamage;
    public int maxHealth;
    public int currentHealth;
    public int maxOvershield;
    public int currentOvershield;
    public float maxStamina;
    public float currentStamina;
    public int enemiesKilled = 0;

    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public OvershieldBar overshieldBar;
    public GameObject controller;

    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine regen;
    
    private void Awake()
    {
        controller = GameObject.Find("Controller");
        maxHealth = controller.GetComponent<BaseStats>().maxHealth;
        currentHealth = controller.GetComponent<BaseStats>().maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        overshieldBar.SetMaxOvershield(maxOvershield);
        overshieldBar.SetOvershield(0);
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetStamina(maxStamina);
        currentStamina = maxStamina;
    }

    // Start is called before the first frame update

    public void SetUp()
    {
        maxHealth = controller.GetComponent<BaseStats>().maxHealth;
        currentHealth = controller.GetComponent<BaseStats>().maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        overshieldBar.SetMaxOvershield(maxOvershield);
        overshieldBar.SetOvershield(0);
    }


    public int SubtractHealth(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        return currentHealth;
    }

    public void AddHealth(int health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            healthBar.SetHealth(maxHealth);
        }
        else
        {
            healthBar.SetHealth(currentHealth);
        }
    }
    
    public void AddOvershield(int overshield)
    {
        currentOvershield += overshield;
        if (currentOvershield > maxOvershield)
        {
            overshieldBar.SetOvershield(maxHealth);
        }
        else
        {
            overshieldBar.SetOvershield(currentOvershield);
        }
    }
    
    public int SubtractOvershield(int damage)
    {
        currentOvershield -= damage;
        overshieldBar.SetOvershield(currentOvershield);
        return currentOvershield;
    }

    public void IncreaseMaxHealth(int health)
    {
        int newHealth = maxHealth + health;
        healthBar.SetMaxHealth(newHealth);
        maxHealth = newHealth;
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina - amount >= 0)
        {
            currentStamina -= amount;
            staminaBar.SetStamina(currentStamina);

            if (regen != null)
            {
                StopCoroutine(regen);
            }
            
            regen = StartCoroutine(RegenStamina());
            return true;
        }
        else
        {
            // Debug.Log("Not enough stamina");
            return false;
        }
    }

    public IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(2);

        while (currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100;
            staminaBar.SetStamina(currentStamina);
            yield return regenTick;
        }

        regen = null;
    }
}
