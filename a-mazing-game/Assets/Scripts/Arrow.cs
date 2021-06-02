using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector3 shootDir;
    [SerializeField] private PlayerStats playerStats;
    
    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    private void Update()
    {
        float shootSpeed = 50f;
        transform.position += shootDir * shootSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(other.GetComponent<AIMovement>().TakeDamage(playerStats.attackDamage));            
            // Debug.Log("hit at " + Time.time);
            // other.TakeDamage(20);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Duck"))
        {
            other.GetComponent<DuckController>().TakeDamage(playerStats.attackDamage);
            // Debug.Log("hit at " + Time.time);
            // other.TakeDamage(20);
            Destroy(gameObject);
        }
    }
}

