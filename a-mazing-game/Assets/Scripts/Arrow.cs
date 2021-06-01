using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector3 shootDir;
    
    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        // Transform player = GameObject.Find("Player").transform;
        // transform.LookAt(player);
        // transform.Rotate(Vector3.up, 90);
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    private void Update()
    {
        float throwSpeed = 40f;
        transform.position += shootDir * throwSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == typeof(BoxCollider))
        {
            PlayerCombat target = other.GetComponent<PlayerCombat>();
            if (target != null)
            {
                // Debug.Log("hit at " + Time.time);
                target.TakePlayerDamage(20);
                Destroy(gameObject);
            }
        }
    }
}
