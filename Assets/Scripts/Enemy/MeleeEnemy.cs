using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    Transform player;
    Transform fishKingdom;
    NavMeshAgent agent;
    public GameObject projectile;
    public GameObject weapon;
    //Transform healthBar;
    public HealthBarScript healthBarScript;

    public LayerMask whatIsPlayer;
    public LayerMask whatIsBuilding;

    //attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //states
    public float sightRange, attackRange, sightRangeNearBuilding;
    private bool playerInSightRange, playerInAttackRange, fishKingdomInSightRange, fishKingdomInAttackRange, playerInSightRangeNearBuilding;

    public float health, maxHealth = 100f;

    void Start()
    {
        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").transform;
        }

        fishKingdom = GameObject.Find("FishKingdom").transform;
        healthBarScript = GetComponentInChildren<HealthBarScript>();
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        fishKingdomInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsBuilding);
        playerInSightRangeNearBuilding = Physics.CheckSphere(transform.position, sightRangeNearBuilding, whatIsPlayer);

        //If player is outside of sight range
        if (!playerInSightRange && !playerInAttackRange)
        {
            HeadToFishKingdom();
        }
        //If player is in sight but not close enough to attack
        if (playerInSightRange && !playerInAttackRange)
        {
            Chasing();
        }
        //If player is close enough to attack
        if (playerInAttackRange && playerInSightRange)
        {
            Attacking(player);
        }
        //If fishKingdom is in attack range but the player isen't in the sight range
        if (fishKingdomInAttackRange && !playerInSightRange)
        {
            Attacking(fishKingdom);
        }
    }

    private void HeadToFishKingdom()
    {
        agent.SetDestination(fishKingdom.position);

        //transform.LookAt(fishKingdom);
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);
    }

    private void Attacking(Transform thing)
    {
        //Stops moving?
        agent.SetDestination(transform.position);

        transform.LookAt(thing);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        healthBarScript.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
