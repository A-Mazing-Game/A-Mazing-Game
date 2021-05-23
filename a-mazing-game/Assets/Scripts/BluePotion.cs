using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BluePotion : InventoryItemBase
{
    public GameObject player;
    private PlayerStats playerStats;
    public Inventory inventory;
    private int potionOvershield = 10;

    
    private void Start()
    {
        playerStats = player.GetComponent<PlayerStats>();
    }
    
    public override void OnUse()
    {
        gameObject.SetActive(true);
        inventory.RemoveItem(this);
        mz.RemoveEnemyNode(gameObject, 1);
        Destroy(gameObject);
        int currentOvershield = playerStats.currentOvershield;
        int maxOvershield = playerStats.maxOvershield;
        if (currentOvershield < maxOvershield)
        {
            if (currentOvershield + potionOvershield >= maxOvershield)
            {
                playerStats.AddOvershield(maxOvershield - currentOvershield);
            }
            else
            {
                playerStats.AddOvershield(potionOvershield);
            }
        }
    }
    
    public override void OnPickup()
    {
        // Destroy(gameObject);
        pickedUp = true;
        gameObject.SetActive(false);
    }
}
