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
    UpgradeJsonHandler.Root root;
    public string audioPath = "TowerDamage";
    public string audioPath2 = "AllySpawn";
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
        if (jsonEmergencyAlly[GetEmergencyAllySpawn()].cost > CurrencyManager.Instance.Currency && GetEmergencyAllySpawn() != 0)
        {
            int costRemaining = jsonEmergencyAlly[GetEmergencyAllySpawn()].cost - CurrencyManager.Instance.Currency;
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("You need " + costRemaining + "$ more"));
            return;
        }
        // Checks if max level reached
        if (GetEmergencyAllySpawn() == 2)
        {
            return;
        }
        // Sets new stats and adds the cost to the money
        allySpawnInTrouble = jsonEmergencyAlly[GetEmergencyAllySpawn()].state;
        // To lazy to actually code the cost so I just made upgrades cost the same amount as placing
        if (GetEmergencyAllySpawn() != 0)
        {
            CurrencyManager.Instance.Currency -= jsonEmergencyAlly[GetEmergencyAllySpawn()].cost;
        }
        AddToMoneySpent(jsonEmergencyAlly[GetEmergencyAllySpawn()].cost);
        SetEmergencyAllySpawn(GetEmergencyAllySpawn() + 1);
    }

    public void TakeDamage(float damage, GameObject whoOwMe = default)
    {
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 0.5f);
        health -= damage;

        getHealthBar().UpdateHealthBar(health, maxHealth);
        float healthPercent = health / maxHealth * 100;

        // Upgrade for FishKingdom to spawn allies when in trouble
        // TODO: Sound Alert?
        if (healthPercent <= 50 && allySpawnInTrouble && toggleOnce < 1)
        {
            for (int i = 0; i < 4; i++)
            {
                SoundFXManager.instance.PrepareSoundFXClip(audioPath2, transform, 0.5f);
                Instantiate(ally, SpawnPointForAlly, transform.rotation);
                
            }
            toggleOnce++;
        }

        if (healthPercent <= 30 && allySpawnInTrouble && toggleOnce < 2)
        {

            for (int i = 0; i < 7; i++)
            {
                SoundFXManager.instance.PrepareSoundFXClip(audioPath2, transform, 0.5f);
                Instantiate(ally, SpawnPointForAlly, transform.rotation);
            }
            toggleOnce++;
        }

        if (health <= 0)
        {
            MenuController.didKingdomDie = true;
            StartCoroutine(PlayParticleAndDisable(true));
        }
    }

    public void OnDestroy()
    {
        // If scene wasn't changing because building got destoryed
        if (MenuController.didKingdomDie)
        {
            ComponentManager.Instance.FocusCursor(false);
            MenuController.didKingdomDie = false;
            SceneManager.LoadScene("LoseScreen");
        }
    }
}