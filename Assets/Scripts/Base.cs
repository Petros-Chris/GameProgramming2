using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Base : MonoBehaviour, IDamageable
{
    private HealthBarScript healthBar;
    NavMeshAgent agent;
    public LayerMask whatIsEnemy;
    public float health;
    public float maxHealth;
    public float attackRange;

    public GameObject bulletToIgnore;

    void Start()
    {
        healthBar = gameObject.GetComponentInChildren<HealthBarScript>();
        
    }

    private void Update()
    {
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
       

        healthBar.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    // for main base to be able to attack back (if we want that)
    /*
    private void Attacking(Transform thing)
    {
            if (!alreadyAttacked)
            {
                if (weapon.TryGetComponent<EnemyGun>(out EnemyGun GunComponemt))
                {
                    GunComponemt.Shoot();
                }
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    */

}
