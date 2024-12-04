using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kingdom : MonoBehaviour, IDamageable
{
    private HealthBarScript healthBar;
    public float health;
    public float maxHealth;
    public GameObject Ally;
    public Vector3 SpawnPointForAlly;
    public bool hasAllySpawnInTroubleUpgrade = false;
    int toggleOnce = 0;

    void Start()
    {
        healthBar = gameObject.GetComponentInChildren<HealthBarScript>();
        SpawnPointForAlly = new Vector3(transform.position.x + -2, transform.position.y, transform.position.z);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        healthBar.UpdateHealthBar(health, maxHealth);
        float healthPercent = health / maxHealth * 100;

        // Upgrade for FishKingdom to spawn allies when in trouble
        // TODO: Sound Alert?
        if (healthPercent <= 50 && hasAllySpawnInTroubleUpgrade && toggleOnce < 1)
        {
            for (int i = 0; i < 4; i++)
            {
                Instantiate(Ally, SpawnPointForAlly, transform.rotation);
            }
            toggleOnce++;
        }

        if (healthPercent <= 30 && hasAllySpawnInTroubleUpgrade && toggleOnce < 2)
        {

            for (int i = 0; i < 7; i++)
            {
                Instantiate(Ally, SpawnPointForAlly, transform.rotation);
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