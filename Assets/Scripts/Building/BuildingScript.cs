using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float health;
    public float maxHealth;
    private VisibleHealthBar healthBar;
    private RegularHealthBar gameOverlayHealthBarKingdom;
    private int healthLevel = 0;
    private int attackLevel = 0;
    private int moneySpent = 0;
    public TowerAttack[] towerAttack; // Cooldown, Range
    public TowerGun[] towerGun; // Damage
    private int emergencyAllySpawn;
    private ParticleSystem spawnParticle;
    private ParticleSystem deathParticle;
    public string audioPath = "TowerDamage";
    public string audioPath3 = "TowerDestroy";

    void OnEnable()
    {
        if (GetSpawnParticle() == null)
        {
            Transform particleTransform = transform.Find("SpawnParticle");

            // Exits if it can't find the particle
            if (particleTransform == null) return;

            SetSpawnParticle(particleTransform.GetComponent<ParticleSystem>());
        }
        GetSpawnParticle().Play();
    }

    public IEnumerator PlayParticleAndDisable(bool destroy = false)
    {
        if (GetDeathParticle() == null)
        {
            SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 0.5f);
            Transform particleTransform = transform.Find("DestoryParticle");

            // Exits if it can't find the particle
            if (particleTransform == null) yield break;

            SetDeathParticle(particleTransform.GetComponent<ParticleSystem>());
        }

        GetDeathParticle().Play();
        yield return new WaitForSeconds(1);

        if (!destroy)
        {
            ComponentManager.Instance.buildingsDisabled.Add(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void Heal(float healAmount)
    {
        health += healAmount;

        GetHealthBar().UpdateHealthBar(health, maxHealth);
        GetGameOverlayHealthBarKingdom().UpdateHealthBar(health, maxHealth);

        if (health >= maxHealth)
        {
            health = maxHealth;
        }
    }

    public bool SetMaxHealth(List<UpgradeJsonHandler.MaxHealth> jsonHealth, RegularHealthBar kingdomHealth = null)
    {
        // Checks if broke
        if (jsonHealth[GetHealthLevel()].cost > CurrencyManager.Instance.Currency && GetHealthLevel() != 0)
        {
            int costRemaining = jsonHealth[GetHealthLevel()].cost - CurrencyManager.Instance.Currency;
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("You need " + costRemaining + "$ more"));
            return false;
        }
        // Checks if max level reached
        if (GetHealthLevel() == jsonHealth[^1].level)
        {
            return false;
        }
        // Sets new stats and adds the cost to the money
        maxHealth = jsonHealth[GetHealthLevel()].maxHealth;
        health = maxHealth; // Sets it to full
        GetHealthBar().UpdateHealthBar(health, maxHealth);
        // Should only update the kingdom health
        if (kingdomHealth != null)
            kingdomHealth.UpdateHealthBar(health, maxHealth);

        if (GetHealthLevel() != 0)
        {
            CurrencyManager.Instance.Currency -= jsonHealth[GetHealthLevel()].cost;
        }
        // Sets new level
        AddToMoneySpent(jsonHealth[GetHealthLevel()].cost);

        SetHealthLevel(GetHealthLevel() + 1);
        return true;
    }

    public bool SetAttack(List<UpgradeJsonHandler.Attack> jsonAttack)
    {
        // Checks if broke
        if (jsonAttack[GetAttackLevel()].cost > CurrencyManager.Instance.Currency && GetAttackLevel() != 0)
        {
            int costRemaining = jsonAttack[GetHealthLevel()].cost - CurrencyManager.Instance.Currency;
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("You need " + costRemaining + "$ more"));
            return false;
        }
        // Checks if max level reached
        if (GetAttackLevel() == jsonAttack[^1].level)
        {
            return false;
        }
        // Sets new stats and adds the cost to the money
        foreach (var tower in towerGun)
        {
            tower.bulletDamage = jsonAttack[GetAttackLevel()].attack;
        }
        foreach (var tower in towerAttack)
        {
            tower.attackCooldown = jsonAttack[GetAttackLevel()].attackSpeed;
        }
        if (GetAttackLevel() != 0)
        {
            CurrencyManager.Instance.Currency -= jsonAttack[GetAttackLevel()].cost;
        }
        // Sets new level
        AddToMoneySpent(jsonAttack[GetAttackLevel()].cost);
        SetAttackLevel(GetAttackLevel() + 1);
        return true;
    }


    // Getters and Setters
    public int GetMoneySpent()
    {
        Debug.Log(moneySpent);
        return moneySpent;

    }

    public void AddToMoneySpent(int amount)
    {
        moneySpent += amount;
    }

    public RegularHealthBar GetGameOverlayHealthBarKingdom()
    {
        return gameOverlayHealthBarKingdom;
    }

    public void SetGameOverlayHealthBarKingdom(RegularHealthBar newHealthBarKingdom)
    {
        gameOverlayHealthBarKingdom = newHealthBarKingdom;
    }

    public int GetEmergencyAllySpawn()
    {
        return emergencyAllySpawn;
    }

    public void SetEmergencyAllySpawn(int localeas)
    {
        emergencyAllySpawn = localeas;
    }

    public VisibleHealthBar GetHealthBar()
    {
        return healthBar;
    }

    public void SetHealthBar(VisibleHealthBar localhealthBar)
    {
        healthBar = localhealthBar;
    }

    public int GetHealthLevel()
    {
        return healthLevel;
    }

    public void SetHealthLevel(int localHealthLevel)
    {
        healthLevel = localHealthLevel;
    }

    public int GetAttackLevel()
    {
        return attackLevel;
    }

    public void SetAttackLevel(int localAttackLevel)
    {
        attackLevel = localAttackLevel;
    }

    public ParticleSystem GetSpawnParticle()
    {
        return spawnParticle;
    }

    public void SetSpawnParticle(ParticleSystem particle)
    {
        spawnParticle = particle;
    }

    public ParticleSystem GetDeathParticle()
    {
        return deathParticle;
    }

    public void SetDeathParticle(ParticleSystem particle)
    {
        deathParticle = particle;
    }
}