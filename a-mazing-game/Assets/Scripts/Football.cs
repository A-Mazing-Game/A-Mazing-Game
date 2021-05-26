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
        transform.eulerAngles = new Vector3(0, GetAngleFromVectorFloat(throwDir), 0);
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

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
