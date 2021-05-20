using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPotion : InventoryItemBase
{
    public GameObject player;
    private PlayerStats playerStats;
    public Inventory inventory;
    private int potionHealth = 20;
    
    private void Start()
    {
        playerStats = player.GetComponent<PlayerStats>();
    }

    public override void OnUse()
    {
        inventory.RemoveItem(this);
        int currentHealth = playerStats.currentHealth;
        int maxHealth = playerStats.maxHealth;
        if (currentHealth < maxHealth)
        {
            if (currentHealth + potionHealth >= maxHealth)
            {
                playerStats.AddHealth(maxHealth - currentHealth);
            }
            else
            {
                playerStats.AddHealth(potionHealth);
            }
        }
    }
    
    public override void OnPickup()
    {
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
