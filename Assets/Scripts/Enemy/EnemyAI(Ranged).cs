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
    public Transform player;
    public Transform building;
    public GameObject weapon;
    public float SightRange = 20f;
    public float maxAngle = 45.0f;
    public float AttackRange = 10f;
    public float attackCooldown = 1f;
    public LayerMask PlayerLayer;
    public LayerMask BuildingLayer;
    public StateType currentState;
    public Transform raycastOrigin;
    public bool alreadyAttacked = false;
    public float health, maxHealth = 100f;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        // Animator = GetComponent<Animator>(); // Commented out since we're not using animations
        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").transform;
        }
        building = GetClosestBuilding();
        Debug.Log("HIYA FROM START");

        StateMachine = new StateMachine();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new PatrolState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackPlayerState(this));
        StateMachine.AddState(new AttackBuildingState(this));

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

            if (angle < maxAngle)
            {
                if (Physics.Raycast(raycastOrigin.position, directionToPlayer, out RaycastHit hit, SightRange))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        //No obstacles in the way
                        return true;
                    }

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

            // Fov of enemy (was missing Rad2Deg)
            float angle = Mathf.Acos(Vector3.Dot(transform.forward, directionToPlayer)) * Mathf.Rad2Deg;

            if (angle < maxAngle)
            {
                if (Physics.Raycast(raycastOrigin.position, directionToPlayer, out RaycastHit hit, AttackRange))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        //No obstacles in the way
                        return true;
                    }
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
    public bool IsBuildingInAttackRange()
    {
        if (building == null)
        {
            return false;
        }
        float distanceToBuilding = Vector3.Distance(transform.position, building.position);
        return distanceToBuilding <= AttackRange;
    }
    public bool CanSeeBuilding()
    {
        if (building == null)
        {
            return false;
        }
        // Direction from NPC to building
        Vector3 directionToBuilding = (building.transform.position - transform.position).normalized;

        // Perform Raycast to check if there's a clear line of sight
        if (Physics.Raycast(raycastOrigin.position, directionToBuilding, out RaycastHit hit, SightRange))
        {
            // Just so the enemy doesn't hit each other for no reason
            if (!hit.transform.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
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

    // Currently will get building closest to enemy,
    // should probably have something that also checks if it is closer to fish kingdom
    // So player doesn't lure a boss with towers (if he can place towers during round)
    public Transform GetClosestBuilding()
    {
        Vector3 enemyPosition = transform.position;
        Collider[] buildingsInRange = Physics.OverlapSphere(enemyPosition, 10000, BuildingLayer);

        if (buildingsInRange.Length == 0)
        {
            Debug.Log("WOOHOO I DID IT");
            return transform;
        }

        Transform closestBuilding = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider building in buildingsInRange)
        {
            float distanceToBuilding = Vector3.Distance(enemyPosition, building.transform.position);

            if (distanceToBuilding < closestDistance)
            {
                closestDistance = distanceToBuilding;
                closestBuilding = building.gameObject.transform;
            }
        }
        return closestBuilding;
    }
}
