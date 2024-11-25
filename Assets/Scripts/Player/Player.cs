using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour, IDamageable
{
    private Transform healthBar;
    public float health;
    public float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.Find("PlayerHealthBar").transform;
        //ComponentManager.deathCam.gameObject.SetActive(false);
    }
    void Update()
    {
        ComponentManager.ToggleBuildPlayerMode();
    }


    public void TakeDamage(float damage)
    {
        health -= damage;
        if (healthBar.TryGetComponent<PlayerHealthBar>(out PlayerHealthBar HealthComponent))
        {
            HealthComponent.UpdateHealthBar(health, maxHealth);
        }
        if (health <= 0)
        {
            //gameObject.SetActive(false);
            Destroy(gameObject);
            ComponentManager.deathCam.gameObject.SetActive(true);
            ComponentManager.playerCam.gameObject.SetActive(false);
        }
    }
}
