using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AllyAI : MonoBehaviour, IDamageable
{
    //Little thing at the front that will look at you and shoot at you (rotates around building)
    public StateMachineAlly StateMachine { get; set; }
    public VisibleHealthBar healthBarScript;
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
    public StateTypeAlly currentState;
    public Animator animator;
    public GameObject fishKingdom;
    private string[] audioPath = {"Damage1","Damage2","Damage1"};
    private string[] audioPath2 = {"Death1","Death2","Death3"};
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        StateMachine = new StateMachineAlly();
        StateMachine.AddState(new PatrolStateAlly(this));
        StateMachine.AddState(new ChaseStateAlly(this));
        StateMachine.AddState(new AttackEnemyState(this));
        fishKingdom = GameObject.Find("FishKingdom");
        healthBarScript.UpdateHealthBar(health, maxHealth);

        StateMachine.TransitionToState(StateTypeAlly.Patrol);
    }

    void Update()
    {
        StateMachine.Update();
        animator.SetFloat("speed", Agent.velocity.magnitude);
        currentState = StateMachine.GetCurrentStateType();
    }

    public Transform GetClosestEnemy(float range)
    {
        Vector3 position = transform.position;
        Collider[] enemiesInRange = Physics.OverlapSphere(position, range, EnemyLayer);

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
    public void Attack(Transform whoToLookAt = default)
    {
        if (!alreadyAttacked)
        {
            if (weapon.TryGetComponent<TowerGun>(out TowerGun GunComponemt))
            {
                if (whoToLookAt != default)
                    Nozzle.LookAt(whoToLookAt);
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
    public void TakeDamage(float damage, GameObject whoOwMe)
    {
        if(SoundFXManager.instance.ChanceToPlaySound(10)){
             SoundFXManager.instance.PrepareSoundFXClipArray(audioPath, transform, 0.5f);
        }
        health -= damage;

        healthBarScript.UpdateHealthBar(health, maxHealth);

        if (currentState == StateTypeAlly.Patrol || currentState == StateTypeAlly.Chase)
        {
            if (enemy == null)
            {
                return;
            }

            if (IsEnemyInRange(SightRange))
            {
                transform.LookAt(whoOwMe.transform);
            }
        }

        if (health <= 0)
        {
           if(SoundFXManager.instance.ChanceToPlaySound(20)){
             SoundFXManager.instance.PrepareSoundFXClipArray(audioPath2, transform, 0.5f);
        }
            Destroy(gameObject);
        }
    }

    public void Heal(float healAmount)
    {
        health += healAmount;

        healthBarScript.UpdateHealthBar(health, maxHealth);

        if (health >= maxHealth)
        {
            health = maxHealth;
        }
    }

    public bool IsSpotReachable(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        Agent.CalculatePath(targetPosition, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }
}