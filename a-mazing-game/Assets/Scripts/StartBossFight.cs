using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossFight : MonoBehaviour
{
    public bool startFight;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startFight = true;
            gameObject.SetActive(false);
        }
    }
}
