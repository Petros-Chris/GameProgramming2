using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AllyAI : MonoBehaviour, IDamageable
{
    //Little thing at the front that will look at you and shoot at you (rotates around building)
    public StateMachineAlly StateMachine { get; set; }
    public HealthBarScript healthBarScript;
    public NavMeshAgent Agent { get; set; }
    public Transform enemy;
    public LayerMask EnemyLayer;
    public Transform raycastOrigin;
    public GameObject weapon;
    public float SightRange = 20f;
    public float maxAngle = 45.0f;
    public float AttackRange = 10f;
    public float attackCooldown = 1f;
    public bool alreadyAttacked = false;
    public float health, maxHealth = 100f;
    public Transform Nozzle;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();

        StateMachine = new StateMachineAlly();
        StateMachine.AddState(new PatrolStateAlly(this));
        StateMachine.AddState(new ChaseStateAlly(this));
        StateMachine.AddState(new AttackEnemyState(this));

        StateMachine.TransitionToState(StateTypeAlly.Patrol);
    }

    void Update()
    {
        StateMachine.Update(); //? Maybe some way to repair this after a script change
        // Animator.SetFloat("CharacterSpeed", Agent.velocity.magnitude);
        //currentState = StateMachine.GetCurrentStateType();
    }

    public Transform GetClosestEnemy()
    {
        Vector3 position = transform.position;
        Collider[] enemiesInRange = Physics.OverlapSphere(position, AttackRange, EnemyLayer);

        if (enemiesInRange.Length == 0)
        {
            return null;
        }

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

    public void LookAt(Transform lookingAt)
    {
        Vector3 direction = (lookingAt.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
        transform.rotation,
        targetRotation,
        Time.deltaTime * 2.0f
        );
    }

    public bool CanSeeEnemy(float rangeMode)
    {
        bool result = false;
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);

        if (distanceToEnemy <= rangeMode)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;

            float angle = Vector3.Angle(transform.forward, directionToEnemy);

            if (angle < maxAngle)
            {
                if (Physics.Raycast(raycastOrigin.position, directionToEnemy, out RaycastHit hit, rangeMode))
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        result = true;
                    }
                }
            }
        }
        return result;
    }

    public bool IsEnemyInRange(float range)
    {
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);
        return distanceToEnemy <= range;
    }
    public void Attack()
    {
        if (!alreadyAttacked)
        {
            if (weapon.TryGetComponent<TowerGun>(out TowerGun GunComponemt))
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