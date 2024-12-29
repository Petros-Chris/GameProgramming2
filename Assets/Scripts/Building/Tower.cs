using UnityEngine;

public class Tower : Building, IDamageable
{
    UpgradeJsonHandler.Root root;

    void Start()
    {
        SetHealthBar(gameObject.GetComponentInChildren<VisibleHealthBar>());
        towerAttack = gameObject.GetComponentsInChildren<TowerAttack>();
        towerGun = gameObject.GetComponentsInChildren<TowerGun>();
        Initialize();
    }
    public void TakeDamage(float damage, GameObject whoOwMe)
    {
        // Ow Sound Effect
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 0.5f);

        health -= damage;
        GetHealthBar().UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            // Death Sound Effect
            SoundFXManager.instance.PrepareSoundFXClip(audioPath3, transform, 0.5f);
            StartCoroutine(PlayParticleAndDisable());
        }
    }

    public void Initialize()
    {
        root = UpgradeJsonHandler.ReadFile();
        var upgrades = root.building.Find(b => b.building == "Tower").upgrades;
        foreach (var upgrade in upgrades)
        {
            if (upgrade.attack.Count != 0)
            {
                SetAttack(upgrade.attack);
            }

            else if (upgrade.maxHealth.Count != 0)
            {
                SetMaxHealth(upgrade.maxHealth);
            }
        }
    }

    public void OnDisable()
    {
        ComponentManager.Instance.buildingsDisabled.Add(gameObject);
    }
}
