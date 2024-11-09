using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Base : MonoBehaviour, IDamageable
{
    private HealthBarScript healthBar;
    NavMeshAgent agent;
    public LayerMask whatIsEnemy;
    public float health;
    public float maxHealth;
    public float attackRange;

    public GameObject bulletToIgnore;

    void Start()
    {
        healthBar = gameObject.GetComponentInChildren<HealthBarScript>();

    }

    private void Update()
    {
    }

    public void TakeDamage(float damage)
    {
        health -= damage;


        healthBar.UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void OnDestroy()
    {
        // Will probably change later on
        if (gameObject.CompareTag("Kingdom"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
