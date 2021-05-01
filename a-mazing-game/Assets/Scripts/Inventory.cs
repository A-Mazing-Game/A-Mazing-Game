using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int numCoins;

    // Start is called before the first frame update
    void Start()
    {
        numCoins = 0;
        
    }


    public void SetUp()
    {
        numCoins = 0;
    }
}
