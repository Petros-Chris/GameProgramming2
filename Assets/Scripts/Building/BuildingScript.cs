using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float health;
    public float maxHealth;
    private HealthBarScript healthBar;
    private int healthLevel = 0;
    private int attackLevel = 0;
    private int moneySpent = 0;
    public TowerAttack[] towerAttack; // Cooldown, Range
    public TowerGun[] towerGun; // Damage
    private int emergencyAllySpawn;

    public bool SetMaxHealth(List<UpgradeJsonHandler.MaxHealth> jsonHealth)
    {
        // Checks if broke
        // if (jsonHealth[healthLevel].cost > CurrencyManager.Instance.Currency)
        // {
        //     return;
        // }
        // Checks if max level reached
        if (GetHealthLevel() == jsonHealth[^1].level)
        {
            return false;
        }
        // Sets new stats and adds the cost to the money
        maxHealth = jsonHealth[GetHealthLevel()].maxHealth;
        health = maxHealth; // Sets it to full
        getHealthBar().UpdateHealthBar(health, maxHealth);

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
        // if (jsonAttack[attackLevel].cost > CurrencyManager.Instance.Currency)
        // {
        //     return;
        // }
        // Checks if max level reached
        Debug.Log(GetAttackLevel());
        Debug.Log(jsonAttack[^1].level);
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

    public int GetMoneySpent()
    {
        Debug.Log(moneySpent);
        return moneySpent;
    }

    public void AddToMoneySpent(int amount)
    {
        moneySpent += amount;
        Debug.Log(moneySpent);
    }

    public int GetEmergencyAllySpawn()
    {
        return emergencyAllySpawn;
    }
    public void SetEmergencyAllySpawn(int localeas)
    {
        emergencyAllySpawn = localeas;
    }
    public HealthBarScript getHealthBar()
    {
        return healthBar;
    }
    public void setHealthBar(HealthBarScript localhealthBar)
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
}