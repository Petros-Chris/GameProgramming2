using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Tower : Building, IDamageable
{
    UpgradeJsonHandler.Root root;

    void Start()
    {
        setHealthBar(gameObject.GetComponentInChildren<HealthBarScript>());
        towerAttack = gameObject.GetComponentsInChildren<TowerAttack>();
        towerGun = gameObject.GetComponentsInChildren<TowerGun>();
        initalize();
    }
    public void TakeDamage(float damage, GameObject whoOwMe)
    {
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 0.5f);
        health -= damage;

        getHealthBar().UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            SoundFXManager.instance.PrepareSoundFXClip(audioPath3, transform, 0.5f);
            StartCoroutine(PlayParticleAndDisable());
        }
    }

    public void initalize()
    {
        root = UpgradeJsonHandler.ReadFile();
        var upgrades = root.building.Find(b => b.building == "Tower").upgrades;
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
        }
    }
    public void OnDisable()
    {
        ComponentManager.Instance.buildingsDisabled.Add(gameObject);
    }
}
