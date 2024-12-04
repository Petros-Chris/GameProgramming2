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
    private float lastDamageTime;
    private bool isRegenerating;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.Find("PlayerHealthBar").transform;
        //ComponentManager.deathCam.gameObject.SetActive(false);
         lastDamageTime = Time.time;
    }
    void Update()
    {
        ComponentManager.ToggleBuildPlayerMode();

        if (!isRegenerating && Time.time - lastDamageTime >= 5f)
        {
            StartCoroutine(RegenerateHealth());
        }
    }


    public void TakeDamage(float damage)
    {
        health -= damage;
        lastDamageTime = Time.time;

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

      private IEnumerator RegenerateHealth()
    {
        isRegenerating = true;

        while (health < maxHealth)
        {
            health += 1;
            health = Mathf.Min(health, maxHealth); 

            if (healthBar.TryGetComponent<PlayerHealthBar>(out PlayerHealthBar HealthComponent))
            {
                HealthComponent.UpdateHealthBar(health, maxHealth);
            }

            yield return new WaitForSeconds(1f);

            if (Time.time - lastDamageTime < 5f)
            {
                isRegenerating = false;
                yield break;
            }
        }

        isRegenerating = false; 
    }
}
