using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIOld : MonoBehaviour
{
    Transform player;
    Transform fishKingdom;
    NavMeshAgent agent;
    public GameObject projectile;
    public GameObject weapon;
    //Transform healthBar;
    public VisibleHealthBar healthBarScript;

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
        if(GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").transform;
        }

        if (GameObject.Find("FishKingdom") != null)
        {
            fishKingdom = GameObject.Find("FishKingdom").transform;
        }
        healthBarScript = GetComponentInChildren<VisibleHealthBar>();
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
    }

    void Update()
    {
        Collider[] buildingsInRange = Physics.OverlapSphere(transform.position, attackRange, whatIsBuilding);

        if (buildingsInRange.Length > 0)
        {
            Transform closestBuilding = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider building in buildingsInRange) 
            {
                float distanceToBuilding = Vector3.Distance(transform.position, building.transform.position);

                if (distanceToBuilding < closestDistance)
                {
                    closestDistance = distanceToBuilding;
                    closestBuilding = building.gameObject.transform;
                }

                agent.SetDestination(closestBuilding.position);
                Attacking(closestBuilding);
                
            }
        } else if(!playerInSightRange && !playerInAttackRange) 
        {
            HeadToFishKingdom();
        }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        fishKingdomInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsBuilding);
        playerInSightRangeNearBuilding = Physics.CheckSphere(transform.position, sightRangeNearBuilding, whatIsPlayer);

        //If player is outside of sight range
        /*
        if (!playerInSightRange && !playerInAttackRange)
        {
            HeadToFishKingdom();
        }
        */
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
        /*
        if (fishKingdomInAttackRange && !playerInSightRange)
        {
            Attacking(fishKingdom);
        }
        */
    }

    private void HeadToFishKingdom()
    {
        if(fishKingdom != null)
        {
            agent.SetDestination(fishKingdom.position);
        }
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
