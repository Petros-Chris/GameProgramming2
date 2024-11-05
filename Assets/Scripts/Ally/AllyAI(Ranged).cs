using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AllyAI : MonoBehaviour, IDamageable
{
    //Little thing at the front that will look at you and shoot at you (rotates around building)
    public StateMachineAlly StateMachine { get; private set; }
    public HealthBarScript healthBarScript;
    public NavMeshAgent Agent { get; private set; }
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


    // Might not have to be separate
    float delayForSeeingEnemy = 0;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Debug.Log("Ally: HIYA FROM START");

        StateMachine = new StateMachineAlly();
        StateMachine.AddState(new IdleStateAlly(this));
        StateMachine.AddState(new PatrolStateAlly(this));
        StateMachine.AddState(new ChaseStateAlly(this));
        StateMachine.AddState(new AttackEnemyState(this));

        StateMachine.TransitionToState(StateTypeAlly.Idle);
    }

    void Update()
    {
        StateMachine.Update(); //? Maybe some way to repair this after a script change
        // Animator.SetFloat("CharacterSpeed", Agent.velocity.magnitude); //? Animation
        //currentState = StateMachine.GetCurrentStateType();
    }

    public bool CanSeeEnemy(float rangeMode)
    {
        bool result = false;
        if (enemy == null)
        {
            StateMachine.TransitionToState(StateTypeAlly.Patrol);
            return result;
        }
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);

        if (distanceToEnemy <= rangeMode)
        {
            // Direction from ally to enemy
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;

            if (Physics.Raycast(raycastOrigin.position, directionToEnemy, out RaycastHit hit, rangeMode))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    delayForSeeingEnemy += Time.deltaTime;
                    if (delayForSeeingEnemy >= 0.5)
                    {
                        result = true;
                    }
                }
                else
                {
                    delayForSeeingEnemy = 0;
                }
            }
            else
            {
                delayForSeeingEnemy = 0;
            }
        }
        return result;
    }
    public Transform GetClosestEnemy()
    {
        Vector3 position = transform.position;
        Collider[] enemiesInRange = Physics.OverlapSphere(position, 10000, EnemyLayer);
        Debug.Log(enemiesInRange);

        if (enemiesInRange.Length == 0)
        {
            Debug.Log("There are no more enemies");
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
    public bool IsEnemyInRange(float range)
    {
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);
        return distanceToEnemy <= range;
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