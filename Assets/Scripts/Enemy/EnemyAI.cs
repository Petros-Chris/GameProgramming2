using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform player;
    Transform fishKingdom;
    NavMeshAgent agent;

    public LayerMask whatIsPlayer;

    //attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //states
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

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

        if (!playerInSightRange && !playerInAttackRange)
        {
            HeadToFishKingdom();
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            Chasing();
        }
        if (playerInAttackRange && playerInSightRange)
        {
            Attacking();
        }
    }

    private void HeadToFishKingdom()
    {
        agent.SetDestination(fishKingdom.position);

        transform.LookAt(fishKingdom);
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);
    }

    private void Attacking()
    {
        //Stops moving?
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {

            // Rigidbody rb = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            // rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            // rb.AddForce(transform.up * 32f, ForceMode.Impulse);

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
