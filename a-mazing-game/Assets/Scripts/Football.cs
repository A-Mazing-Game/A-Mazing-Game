using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Football : MonoBehaviour
{
    private Vector3 throwDir;
    
    public void Setup(Vector3 throwDir)
    {
        this.throwDir = throwDir;
        transform.eulerAngles = this.throwDir;
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    private void Update()
    {
        float throwSpeed = 20f;
        transform.position += throwDir * throwSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCombat target = other.GetComponent<PlayerCombat>();
        if (target != null)
        {
            target.TakePlayerDamage(30);
            Destroy(gameObject);
        }
    }
}
