using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    private Transform healthBar;
    public float health;
    public float maxHealth;
    private float lastDamageTime;
    private bool isRegenerating;

    [Header("Revival")]
    public float reviveTime = 5f;
    private bool isDead = false;
    private float revivalCountdown;
    public KeyCode reviveKey = KeyCode.R;

    private int revivalCount = 0;
    public int maxRevives = 3;

    public Slider healthSlider;
    public Text revivalTimerText;
    public Text deathMessageText;


    void Start()
    {
        healthSlider = GameObject.Find("PlayerHealthBar").GetComponent<Slider>();

        lastDamageTime = Time.time;
        revivalCountdown = reviveTime;
    }

    void Update()
    {
        if (!isRegenerating && Time.time - lastDamageTime >= 5f)
        {
            StartCoroutine(RegenerateHealth());
        }

        if (isDead)
        {
            if (revivalCountdown > 0)
            {
                revivalCountdown -= Time.deltaTime;
                if (revivalTimerText != null)
                {
                    revivalTimerText.text = "Reviving in " + Mathf.Ceil(revivalCountdown) + " seconds";
                }
            }
            else
            {
                Revive();
            }

            if (Input.GetKeyDown(reviveKey) && revivalCount < maxRevives)
            {
                Revive();
            }
        }
    }

    public void TakeDamage(float damage, GameObject whoOwMe = default)
    {
        if (isDead) return;

        health -= damage;
        lastDamageTime = Time.time;

        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        revivalCountdown = reviveTime;
        health = 0; // Ensure the player's health is set to 0 on death

        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        // Disable player movement and controls
        ComponentManager.Instance.hasPlayerDied = true;
        ComponentManager.Instance.deathCam.gameObject.SetActive(true);
        ComponentManager.Instance.playerCam.gameObject.SetActive(false);

        if (deathMessageText != null)
        {
            deathMessageText.gameObject.SetActive(true);
        }

        Debug.Log("Player has died. Reviving in " + reviveTime + " seconds...");
    }

    private void Revive()
    {
        if (isDead && revivalCount < maxRevives)
        {
            health = maxHealth / 2;
            if (healthSlider != null)
            {
                healthSlider.value = health;
            }

            revivalCount++;

            // If max revives have been reached, stop further revives
            if (revivalCount >= maxRevives)
            {
                Debug.Log("You have reached the maximum number of revives.");
                if (deathMessageText != null)
                {
                    deathMessageText.text = "Game Over!";
                }
                SceneManager.LoadScene("LoseScreen");
            }
            else
            {
                isDead = false;
                revivalCountdown = reviveTime;

                // Switch back to player camera
                ComponentManager.Instance.deathCam.gameObject.SetActive(false);
                ComponentManager.Instance.playerCam.gameObject.SetActive(true);

                if (deathMessageText != null)
                {
                    deathMessageText.gameObject.SetActive(false);
                }

                Debug.Log("Player revived! Revival count: " + revivalCount);
            }
        }
        else if (revivalCount >= maxRevives)
        {
            Debug.Log("Player cannot be revived anymore.");
        }
    }

    private IEnumerator RegenerateHealth()
    {
        isRegenerating = true;

        while (health < maxHealth)
        {
            health += 1;
            health = Mathf.Min(health, maxHealth);

            if (healthSlider != null)
            {
                healthSlider.value = health;
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