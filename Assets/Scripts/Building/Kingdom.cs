using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kingdom : Building, IDamageable
{
    private GameObject ally;
    private Vector3 SpawnPointForAlly;
    public bool hasAllySpawnInTroubleUpgrade = false;
    private int toggleOnce = 0;
    private Dictionary<string, int> methodMap;

    private int maxHealthCount = 1;
    UpgradeJsonHandler.Root root;


    void Start()
    {
        ally = ComponentManager.Instance.defaultAlly;
        setHealthBar(gameObject.GetComponentInChildren<HealthBarScript>());
        SpawnPointForAlly = new Vector3(transform.position.x, transform.position.y, transform.position.z + -2);
        root = UpgradeJsonHandler.ReadFile();
        Debug.Log("ASD");

        methodMap = new Dictionary<string, int>
        {
            { "maxHealth", 1 }
        };
        Debug.Log("ASD");
    }

    public void UpgradeMaxHealth()
    {
        if (maxHealthCount == 5)
        {
            return;
        }
        maxHealth += 600;

        health = maxHealth; // Sets it to full
        //health += 600; // Only adds the new health onto the original
        getHealthBar().UpdateHealthBar(health, maxHealth);

        maxHealthCount++;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        getHealthBar().UpdateHealthBar(health, maxHealth);
        float healthPercent = health / maxHealth * 100;

        // Upgrade for FishKingdom to spawn allies when in trouble
        // TODO: Sound Alert?
        if (healthPercent <= 50 && hasAllySpawnInTroubleUpgrade && toggleOnce < 1)
        {
            for (int i = 0; i < 4; i++)
            {
                Instantiate(ally, SpawnPointForAlly, transform.rotation);
            }
            toggleOnce++;
        }

        if (healthPercent <= 30 && hasAllySpawnInTroubleUpgrade && toggleOnce < 2)
        {

            for (int i = 0; i < 7; i++)
            {
                Instantiate(ally, SpawnPointForAlly, transform.rotation);
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