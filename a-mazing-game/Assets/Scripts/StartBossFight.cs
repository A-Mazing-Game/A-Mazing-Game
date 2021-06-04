using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossFight : MonoBehaviour
{
    public bool startFight;
    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        if (other.CompareTag("Player"))
            startFight = true;
    }
}
