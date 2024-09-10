using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform player;
    Transform fishKingdom;
    NavMeshAgent agent;
    public GameObject projectile;
    public GameObject weapon;

    public LayerMask whatIsPlayer;
    public LayerMask whatIsBuilding;

    //attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //states
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange, fishKingdomInSightRange, fishKingdomInAttackRange;

    public float health, maxHealth = 100f;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        fishKingdom = GameObject.Find("FishKingdom").transform;
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        fishKingdomInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsBuilding);

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
        //If fishKingdom is in attack range
        if (fishKingdomInAttackRange)
        {
            Attacking(fishKingdom);
        }

    }

    private void HeadToFishKingdom()
    {
        agent.SetDestination(fishKingdom.position);

        //transform.LookAt(fishKingdom);
    }

    private void AttackFishKingdom()
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
            if (weapon.gameObject.TryGetComponent<EnemyGun>(out EnemyGun GunComponemt))
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
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
