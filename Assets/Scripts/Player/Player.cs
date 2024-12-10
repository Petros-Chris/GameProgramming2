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
        ComponentManager.Instance.ToggleBuildPlayerMode();

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
            Destroy(gameObject);
            ComponentManager.Instance.hasPlayerDied = true;
            // Switch cameras
            ComponentManager.Instance.deathCam.gameObject.SetActive(true);
            ComponentManager.Instance.playerCam.gameObject.SetActive(false); // If we change the player to have camera on body, this should be removed else it will make error
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

// Gun laggy (sanppy)
// It feels like the enemies focus on me more than the towers 
// the floaty feel on buildings feels wrong (should act like im on ground but without the whatIsGround layer)
// If im at certain spots, the enemies freak out (start pacing back and forth) (it's likely because they don't see me anymore, so i just have to make them go to last spot of where i was before forgetting about me)
// Having towers automatically come back feels kinda op right now (likely because of lack of upgrading and the weak enemy we fight against)
// Tower Reparability is missing, it just comes back low hp
// buildings just kind of popping in is suddenly perhaps there should be partcles of the building getting rebuilt (like particles begian than 3 seconds later they would appear and particles dissaper)