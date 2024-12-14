using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Wall : Building
{
    UpgradeJsonHandler.Root root;
    public GameObject gate;
    public Transform ally;
    Vector3 openPos = new Vector3(0, 0.3f, 0);
    Vector3 openScale = new Vector3(0.5f, 1f, 4f);
    Vector3 closedPos = new Vector3(0, -0.98f, 0);
    Vector3 closedScale = new Vector3(0.5f, 3.6f, 4f);

    void Start()
    {
        gate = transform.Find("Mesh/Gate").gameObject;
        towerAttack = gameObject.GetComponentsInChildren<TowerAttack>();
        towerGun = gameObject.GetComponentsInChildren<TowerGun>();


    }
    void Update()
    {
        OpenGate();
    }

    public void initalize()
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

    public void OpenGate()
    {
        ally = GetClosestAlly(30);

        if (ally == null)
        {
            Debug.Log("AA");
            if (gate.transform.localScale == openScale)
            {
                Debug.Log("BB");
                gate.transform.localScale = closedScale;
                gate.transform.localPosition = closedPos;
            }
            return;
        }

        if (IsAllyInRange(5, ally))
        {
            Debug.Log("ccc");
            gate.transform.localScale = openScale;
            gate.transform.localPosition = openPos;
        }
        else
        {
            Debug.Log("ddd");
            gate.transform.localScale = closedScale;
            gate.transform.localPosition = closedPos;
        }
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