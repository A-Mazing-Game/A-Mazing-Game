using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStats : MonoBehaviour
{
    public int attackDamage = 10;
    public int maxHealth = 50;
    public int currentHealth;

    //public IDictionary<string, int> baseStats = new Dictionary<string, int>();
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseHealth()
    {
        maxHealth++;
    }

    public void IncreaseAttack()
    {
        attackDamage++;
    }
}
