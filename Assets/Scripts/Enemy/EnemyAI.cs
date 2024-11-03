using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public HealthBarScript healthBarScript;
    public StateMachine StateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    // public Animator Animator { get; private set; } // Not needed since we're not using animations
    public Transform[] Waypoints;
    public Transform player;
    public Transform fishKingdom;
    public GameObject weapon;
    public float SightRange = 20f;
    public float maxAngle = 45.0f;
    public float AttackRange = 10f; // New attack range variable
    public float attackCooldown = 1f;
    public LayerMask PlayerLayer;
    public LayerMask obstacleMask; // Assign this in the Inspector to include walls, terrain, etc.
    public StateType currentState;
    public Transform raycastOrigin;
    public bool alreadyAttacked = false;
    public float health, maxHealth = 100f;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        // Animator = GetComponent<Animator>(); // Commented out since we're not using animations
        if (GameObject.Find("FishKingdom") != null)
        {
            fishKingdom = GameObject.Find("FishKingdom").transform;
        }
        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").transform;
        }

        StateMachine = new StateMachine();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new PatrolState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this));

        StateMachine.TransitionToState(StateType.Idle);
    }

    void Update()
    {
        StateMachine.Update();
        // Animator.SetFloat("CharacterSpeed", Agent.velocity.magnitude); //? Animation
        //currentState = StateMachine.GetCurrentStateType();
    }

    //* From what i can understand, the methods below are used by the states, so this class is more of a abstract class but with methods already filled out

    public bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= SightRange)
        {
            // Direction from NPC to player
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float angle = Mathf.Acos(Vector3.Dot(transform.forward, directionToPlayer));

            // Im not sure i understand this
            if (angle < maxAngle)
            {
                Debug.DrawRay(raycastOrigin.position, directionToPlayer * SightRange, Color.red);

                // // Perform Raycast to check if there's a clear line of sight
                if (Physics.Raycast(raycastOrigin.position, directionToPlayer, SightRange, LayerMask.GetMask("whatIsPlayer")))
                {
                    //No obstacles in the way
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanSeePlayerWhileAttacking()
    {
        if (player == null)
        {
            StateMachine.TransitionToState(StateType.Patrol);
            return false;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= AttackRange)
        {
            // Direction from NPC to player
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float angle = Mathf.Acos(Vector3.Dot(transform.forward, directionToPlayer));

            // Im not sure i understand this
            if (angle < maxAngle)
            {
                Debug.DrawRay(raycastOrigin.position, directionToPlayer * AttackRange, Color.red);

                // // Perform Raycast to check if there's a clear line of sight
                if (Physics.Raycast(raycastOrigin.position, directionToPlayer, AttackRange, LayerMask.GetMask("whatIsPlayer")))
                {
                    //No obstacles in the way
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsPlayerInAttackRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= AttackRange;
    }
    public void Attack()
    {
        if (!alreadyAttacked)
        {
            if (weapon.TryGetComponent<EnemyGun>(out EnemyGun GunComponemt))
            {
                GunComponemt.Shoot();
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    /// <summary>
    /// Enemy Takes damage
    /// </summary>
    /// <param name="damage">How much damage the enemy is going to take</param>
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
