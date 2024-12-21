using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private Transform healthBar;
    public float health;
    public float maxHealth;
    private float lastDamageTime;
    private bool isRegenerating;

    private string[] audioPath = { "Damage1", "Damage2", "Damage1" };
    private string[] audioPath2 = { "Death1", "Death2", "Death3" };

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.Find("PlayerHealthBar").transform;
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

    public void TakeDamage(float damage, GameObject whoOwMe = default)
    {
        if (SoundFXManager.instance.chancePlaySound(3))
        {
            SoundFXManager.instance.PrepareSoundFXClipArray(audioPath, transform, 0.5f);
        }
        health -= damage;
        lastDamageTime = Time.time;

        if (healthBar.TryGetComponent<PlayerHealthBar>(out PlayerHealthBar HealthComponent))
        {
            HealthComponent.UpdateHealthBar(health, maxHealth);
        }
        if (health <= 0)
        {
            SoundFXManager.instance.PrepareSoundFXClipArray(audioPath2, transform, 0.5f);
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