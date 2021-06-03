using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<NavMeshAgent>().isStopped = true;
            StartCoroutine(other.GetComponent<AIMovement>().TakeDamage(playerStats.attackDamage));
            yield return new WaitForSeconds(0.5f);
            other.GetComponent<NavMeshAgent>().isStopped = false;
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

