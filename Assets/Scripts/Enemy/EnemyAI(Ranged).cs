using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour, IDamageable
{
    public HealthBarScript healthBarScript;
    public StateMachine StateMachine { get; set; }
    public NavMeshAgent Agent { get; set; }
    // public Animator Animator { get; private set; } // Not needed since we're not using animations
    public Transform ally;
    public Transform building;
    public GameObject weapon;
    public float SightRange = 20f;
    public float maxAngle = 45.0f;
    public float AttackRange = 10f;
    public float attackCooldown = 1f;
    public LayerMask EnemyLayer;
    public LayerMask BuildingLayer;
    public StateType currentState;
    public Transform raycastOrigin;
    public bool alreadyAttacked = false;
    public float health, maxHealth = 100f;
    public int value = 3;
    public Transform Nozzle;

    public float thinkingSpeed = 0.5f;


    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        // Animator = GetComponent<Animator>(); // Commented out since we're not using animations
        building = GetClosestBuilding();
        ally = GetClosestEnemy();
        StateMachine = new StateMachine();
        StateMachine.AddState(new HeadToTowerState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackPlayerState(this));
        StateMachine.AddState(new AttackBuildingState(this));

        StateMachine.TransitionToState(StateType.HeadToTower);
        // InvokeRepeating("UpdateStateMachine", 0f, 0.5f);
    }
    void Update()
    {
        //Animator.SetFloat("CharacterSpeed", Agent.velocity.magnitude); //? Animation
        // if (StateMachine == null)
        // {
        //     Start();
        // }
        // else
        // {
        StateMachine.Update();
        // }
        currentState = StateMachine.GetCurrentStateType();
    }

    void UpdateStateMachine()
    {
        if (StateMachine == null)
        {
            Start();
        }
        else
        {
            StateMachine.Update();
            currentState = StateMachine.GetCurrentStateType();
        }
    }

    public Transform GetClosestEnemy()
    {
        Vector3 position = transform.position;
        Collider[] enemiesInRange = Physics.OverlapSphere(position, 10000, EnemyLayer);

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector3.Distance(position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.gameObject.transform;
            }
        }
        return closestEnemy;
    }

    public void LookAt(Transform thing)
    {
        Vector3 direction = (thing.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
        transform.rotation,
        targetRotation,
        Time.deltaTime * 1.25f
        );
    }

    // Currently will get building closest to enemy,
    // should probably have something that also checks if it is closer to fish kingdom
    public Transform GetClosestBuilding()
    {
        Vector3 enemyPosition = transform.position;
        Collider[] buildingsInRange = Physics.OverlapSphere(enemyPosition, 10000, BuildingLayer);

        if (buildingsInRange.Length == 0)
        {
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

    public bool CanSeeEnemy(float rangeMode)
    {
        bool result = false;
        float distanceToPlayer = Vector3.Distance(transform.position, ally.position);

        if (distanceToPlayer <= rangeMode)
        {
            // Direction from NPC to player
            Vector3 directionToEnemy = (ally.transform.position - transform.position).normalized;

            float angle = Vector3.Angle(transform.forward, directionToEnemy);

            if (angle < maxAngle)
            {
                if (Physics.Raycast(raycastOrigin.position, directionToEnemy, out RaycastHit hit, rangeMode))
                {
                    if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Ally"))
                    {
                        result = true;
                    }
                }
            }
        }
        return result;
    }

    public bool CanSeeBuilding()
    {
        bool result = false;
        if (building == null)
        {
            return result;
        }
        // Direction from NPC to building
        Vector3 directionToBuilding = (building.transform.position - transform.position).normalized;

        // Perform Raycast to check if there's a clear line of sight
        if (Physics.Raycast(raycastOrigin.position, directionToBuilding, out RaycastHit hit, SightRange))
        {
            // Just so the enemy doesn't hit each other for no reason
            if (!hit.transform.CompareTag("Enemy"))
            {
                result = true;
            }
        }
        return result;
    }

    public bool IsEnemyInRange(float range)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, ally.position);
        return distanceToPlayer <= range;
    }

    public bool IsPlayerInAttackRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, ally.position);
        return distanceToPlayer <= AttackRange;
    }

    public bool IsBuildingInRange(float range)
    {
        float distanceToBuilding = Vector3.Distance(transform.position, building.position);
        return distanceToBuilding <= range;
    }

    public void Attack(Transform whoToLookAt)
    {
        if (!alreadyAttacked)
        {
            if (weapon.TryGetComponent<EnemyGun>(out EnemyGun GunComponent))
            {
                // Vector3 targetPosition = ally.transform.position;

                //weapon.transform.LookAt(new Vector3(ally.position.x, transform.position.y, transform.position.z));
                Nozzle.LookAt(whoToLookAt);
                GunComponent.Shoot();
            }
            else if (weapon.TryGetComponent<EnemyMeleeWeapon>(out EnemyMeleeWeapon meleeWeapon))
            {
                meleeWeapon.Stab(AttackRange);
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }
    public void ResetAttack()
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
            CurrencyManager.Instance.Currency += value;
            Destroy(gameObject);
        }
    }
}
