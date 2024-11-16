using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Base : MonoBehaviour, IDamageable
{
    private HealthBarScript healthBar;
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
            Destroy(gameObject);
        }
    }
    public void OnDestroy()
    {
        // Will probably change later on
        if (!MenuController.isSceneChanging && gameObject.CompareTag("Kingdom"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            MenuController.isSceneChanging = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (gameObject.CompareTag("Kingdom"))
        {
            MenuController.isSceneChanging = false;
        }
    }
}
