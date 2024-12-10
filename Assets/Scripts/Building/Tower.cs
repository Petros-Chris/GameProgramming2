using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Tower : Building, IDamageable
{
    private int maxHealthCount = 1;

    void Start()
    {
        setHealthBar(gameObject.GetComponentInChildren<HealthBarScript>());
    }
    public void TakeDamage(float damage)
    {
        health -= damage;

        getHealthBar().UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void UpgradeMaxHealth()
    {
        if (maxHealthCount == 5)
        {
            return;
        }
        maxHealth += 400;

        health = maxHealth; // Sets it to full
        //health += 400; // Only adds the new health onto the original
        getHealthBar().UpdateHealthBar(health, maxHealth);

        maxHealthCount++;
    }


    public void OnDisable()
    {
        ComponentManager.Instance.TowersDisabled.Add(gameObject);
    }
}
