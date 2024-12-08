using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Tower : MonoBehaviour, IDamageable
{
    public HealthBarScript healthBar;
    public float health;
    public float maxHealth;

    void Start()
    {
        healthBar = gameObject.GetComponentInChildren<HealthBarScript>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        healthBar.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnDestroy()
    {

    }



    public void OnDisable()
    {
        ComponentManager.Instance.TowersDisabled.Add(gameObject);
    }
}
