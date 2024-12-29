using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private RegularHealthBar healthBar;
    public float health;
    public float maxHealth;
    public int chanceIMakeOwSound = 3;
    public float regenRate = 1f; // The rate player regains health
    public float timeToBeginRecovery = 5f; // How long until the healing begins
    private bool isRegenerating;
    private float progressToBeginRecovery; // The timer to began recovery
    private string[] owSoundEffects = { "Damage1", "Damage2", "Damage1" };
    private string[] deathSoundEffects = { "Death1", "Death2", "Death3" };
    private string respawnSoundEffect = "PlayerRespawn";

    void Start()
    {
        healthBar = GameObject.Find("PlayerSlider").gameObject.GetComponent<RegularHealthBar>();
        // TODO: Player upgrade system
        // healthBar.UpdateHealthBar(health, maxHealth);
        isRegenerating = true;
        StartCoroutine(RegenerateHealth());
    }

    void Update()
    {
        ComponentManager.Instance.ToggleBuildPlayerMode();
    }

    public void TakeDamage(float damage, GameObject whoOwMe = default)
    {
        health -= damage;
        healthBar.UpdateHealthBar(health, maxHealth);
        // Chance To play a sound effect when hit
        if (SoundFXManager.instance.ChanceToPlaySound(chanceIMakeOwSound))
        {
            SoundFXManager.instance.PrepareSoundFXClipArray(owSoundEffects, transform, 0.5f);
        }
        // If the player has no more health
        if (health <= 0)
        {
            ComponentManager.Instance.hasPlayerDied = true;
            StopAllCoroutines();
            Destroy(gameObject);
            // Create sound effect
            SoundFXManager.instance.PrepareSoundFXClipArray(deathSoundEffects, transform, 0.5f);
            // Switch cameras
            ComponentManager.Instance.deathCam.gameObject.SetActive(true);
            ComponentManager.Instance.playerCam.gameObject.SetActive(false);
        }
        isRegenerating = false;
        progressToBeginRecovery = 0;
    }

    private IEnumerator RegenerateHealth()
    {
        // While the player is allowed to recover
        while (isRegenerating)
        {
            // If fully healed
            if (health >= maxHealth)
            {
                isRegenerating = false;
                yield return null;
                continue;
            }
            health += 1;
            healthBar.UpdateHealthBar(health, maxHealth);
            yield return new WaitForSeconds(regenRate);
        }
        StartCoroutine(CountdownToBeginRecovery());
    }

    private IEnumerator CountdownToBeginRecovery()
    {
        // While the player is not allowed to recover
        while (!isRegenerating)
        {
            if (health >= maxHealth)
            {
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(1f);
            progressToBeginRecovery += 1;
            // If timer is reached begin healing the player again
            if (progressToBeginRecovery == timeToBeginRecovery)
            {
                isRegenerating = true;
                progressToBeginRecovery = 0;
            }
        }
        StartCoroutine(RegenerateHealth());
    }

    void OnEnable()
    {
        if (ComponentManager.Instance.hasPlayerDied)
        {
            SoundFXManager.instance.PrepareSoundFXClip(respawnSoundEffect, transform, 0.5f, true);
            ComponentManager.Instance.hasPlayerDied = false;
        }
    }
}