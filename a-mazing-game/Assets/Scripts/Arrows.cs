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
        isDropped = true;
        arrow.SetActive(false);
        gameObject.SetActive(true);
        mz.arrowList.AddLast(gameObject);
        gameObject.transform.rotation = Quaternion.Euler(DropRotation);
    }

    public override void OnPickup()
    {
        isDropped = false;
        mz.RemoveEnemyNode(gameObject, 2);
        Vector3 temp = new Vector3(0, -10, 0);
        gameObject.transform.position += temp;
        gameObject.SetActive(false);
    }
}
