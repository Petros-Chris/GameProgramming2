using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private Transform healthBar;
    public float health;
    public float maxHealth;
    public Camera deathCam;
    public Camera playerCam;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.Find("PlayerHealthBar").transform;
       // deathCam = GameObject.Find("Death Cam");
        deathCam.gameObject.SetActive(false);
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
            deathCam.gameObject.SetActive(true);
            playerCam.gameObject.SetActive(false);
        }
    }
}
