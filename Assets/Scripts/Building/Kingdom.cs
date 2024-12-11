using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kingdom : Building, IDamageable
{
    private GameObject ally;
    private Vector3 SpawnPointForAlly;
    public bool allySpawnInTrouble = false;
    private int toggleOnce = 0;
    private int emergencyAllySpawn = 0;
    UpgradeJsonHandler.Root root;

    void Start()
    {
        ally = ComponentManager.Instance.defaultAlly;
        setHealthBar(gameObject.GetComponentInChildren<HealthBarScript>());
        SpawnPointForAlly = new Vector3(transform.position.x, transform.position.y, transform.position.z + -2);
        towerAttack = gameObject.GetComponentsInChildren<TowerAttack>();
        towerGun = gameObject.GetComponentsInChildren<TowerGun>();
        initalize();
    }

    public void initalize()
    {
        root = UpgradeJsonHandler.ReadFile();
        var upgrades = root.building.Find(b => b.building == "FishKingdom").upgrades;
        foreach (var upgrade in upgrades)
        {
            // It only runs one :O
            if (upgrade.attack.Count != 0)
            {
                SetAttack(upgrade.attack);
            }

            else if (upgrade.maxHealth.Count != 0)
            {
                // Set it as the first level
                SetMaxHealth(upgrade.maxHealth);
            }

            else if (upgrade.emergencyAllySpawn.Count != 0)
            {
                SetEmergencyAllySpawn(upgrade.emergencyAllySpawn);
            }
        }
    }
    public void SetEmergencyAllySpawn(List<UpgradeJsonHandler.EmergencyAllySpawn> jsonEmergencyAlly)
    {
        // Checks if broke
        // if (jsonEmergencyAlly[emergencyAllySpawn].cost > CurrencyManager.Instance.Currency)
        // {
        //     return;
        // }
        // Checks if max level reached
        if (emergencyAllySpawn == 1)
        {
            return;
        }
        // Sets new stats and adds the cost to the money
        Debug.Log("Asdasd" + jsonEmergencyAlly[emergencyAllySpawn].state);
        allySpawnInTrouble = jsonEmergencyAlly[emergencyAllySpawn].state;
        // CurrencyManager.Instance.Currency -= jsonEmergencyAlly[emergencyAllySpawn].cost;
        // Sets new level
        emergencyAllySpawn++;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        getHealthBar().UpdateHealthBar(health, maxHealth);
        float healthPercent = health / maxHealth * 100;

        // Upgrade for FishKingdom to spawn allies when in trouble
        // TODO: Sound Alert?
        if (healthPercent <= 50 && allySpawnInTrouble && toggleOnce < 1)
        {
            for (int i = 0; i < 4; i++)
            {
                Instantiate(ally, SpawnPointForAlly, transform.rotation);
            }
            toggleOnce++;
        }

        if (healthPercent <= 30 && allySpawnInTrouble && toggleOnce < 2)
        {

            for (int i = 0; i < 7; i++)
            {
                Instantiate(ally, SpawnPointForAlly, transform.rotation);
            }
            toggleOnce++;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void OnDestroy()
    {
        if (!MenuController.isSceneChanging)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            MenuController.isSceneChanging = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}