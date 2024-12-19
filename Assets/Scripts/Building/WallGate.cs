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
    public GameObject gate;

    void Start()
    {
        Debug.Log("Aa");
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
            StartCoroutine(PlayParticleAndDisable());
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

    public void GateHandler()
    {
        ally = GetClosestAlly(5);

        if (ally == null)
        {
            CloseGate();
        }
        else
        {
            OpenGate();
        }
        return;
    }

    public void CloseGate()
    {
        gate.transform.localScale = Vector3.Lerp(gate.transform.localScale, closedScale, 0.05f);
        gate.transform.localPosition = Vector3.Lerp(gate.transform.localPosition, closedPos, 0.05f);
    }

    public void OpenGate()
    {
        gate.transform.localScale = Vector3.Lerp(gate.transform.localScale, openScale, 0.05f);
        gate.transform.localPosition = Vector3.Lerp(gate.transform.localPosition, openPos, 0.05f);
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
