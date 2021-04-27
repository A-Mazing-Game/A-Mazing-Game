using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public CapsuleCollider enemy;

    private bool enemyHit;
    private void OnCollisionEnter(Collision other)
    {
        enemyHit = true;
        Debug.Log("enemy hit");
    }
}
