using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrows : InventoryItemBase
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private Inventory inventory;
    public int pickupAmount;

    private void Start()
    {
        pickupAmount = 5;
    }
    
    public override void OnUse()
    {
        gameObject.SetActive(false);
        arrow.SetActive(true);
    }

    public override void OnDrop()
    {
        arrow.SetActive(false);
        gameObject.SetActive(true);
        gameObject.transform.rotation = Quaternion.Euler(DropRotation);
    }
}