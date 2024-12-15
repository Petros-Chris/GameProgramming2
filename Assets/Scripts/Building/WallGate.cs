using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallGate : Wall, IDamageable
{
    Vector3 openPos = new Vector3(0, 0.3f, 0);
    Vector3 openScale = new Vector3(0.5f, 1f, 4f);
    Vector3 closedPos = new Vector3(0, -0.98f, 0);
    Vector3 closedScale = new Vector3(0.5f, 3.6f, 4f);
    UpgradeJsonHandler.Root root;

    void Start()
    {
        towerAttack = gameObject.GetComponentsInChildren<TowerAttack>();
        towerGun = gameObject.GetComponentsInChildren<TowerGun>();
        setHealthBar(gameObject.GetComponentInChildren<HealthBarScript>());
        Initalize();
        // InvokeRepeating("OpenGate", 1, 1);
    }

    void Update()
    {
        GateHandler();
    }

    public void TakeDamage(float damage, GameObject whoOwMe = null)
    {
        health -= damage;

        getHealthBar().UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void Initalize()
    {
        root = UpgradeJsonHandler.ReadFile();
        var upgrades = root.building.Find(b => b.building == "Wall").upgrades;
        foreach (var upgrade in upgrades)
        {
            if (upgrade.maxHealth.Count != 0)
            {
                // Set it as the first level
                SetMaxHealth(upgrade.maxHealth);
            }
        }
    }

    void OnDestroy()
    {
        GameObject parent = gameObject.transform.parent.gameObject;
        Destroy(parent);
    }

    public void GateHandler()
    {
        ally = GetClosestAlly(30);

        if (ally == null)
        {
            if (transform.localScale == openScale)
            {
                CloseGate();
            }
            return;
        }

        if (IsAllyInRange(5, ally))
        {
            OpenGate();
        }
        else if (!IsAllyInRange(7, ally))
        {
            CloseGate();
        }
        return;
    }

    public void CloseGate()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, closedScale, 2 * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, closedPos, 2 * Time.deltaTime);
    }
    public void OpenGate()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, openScale, 2 * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, openPos, 2 * Time.deltaTime);
    }

    public void OnDisable()
    {
        ComponentManager.Instance.buildingsDisabled.Add(gameObject);
    }

    public bool IsAllyInRange(float range, Transform ally)
    {
        float distanceToEnemy = Vector3.Distance(transform.position, ally.position);
        return distanceToEnemy <= range;
    }

    public Transform GetClosestAlly(float range)
    {
        Vector3 position = transform.position;
        Collider[] alliesInRange = Physics.OverlapSphere(position, range, LayerMask.GetMask("whatIsAlly"));

        if (alliesInRange.Length == 0)
        {
            return null;
        }

        Transform closestAlly = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider ally in alliesInRange)
        {
            float distanceToEnemy = Vector3.Distance(position, ally.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestAlly = ally.gameObject.transform;
            }
        }
        return closestAlly;
    }
}
