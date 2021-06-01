using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAndArrow : InventoryItemBase
{
    [SerializeField] private GameObject arrow;
    
    public override void OnUse()
    {
        transform.localPosition = PickPosition;
        transform.localEulerAngles = PickRotation;
        // arrow.SetActive(true);
    }
    
    public override void OnDrop()
    {
        base.OnDrop();
    }
}
