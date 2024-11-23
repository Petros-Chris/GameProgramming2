using System.Collections;
using UnityEngine;

public class Gun2 : MonoBehaviour
{
    public GameObject ExplosionEffect; // Prefab for the explosion visual effect
    public Transform FirePoint; // Point from where the gun fires
    public float range = 100f; // Max range of the gun
    public float explosionRadius = 5f; // Radius of the explosion
    public float damage = 20f; // Damage dealt to enemies within the explosion radius
    public float fireCooldown = 5f; // Cooldown between shots
    public float bulletRegenTime = 10f; // Time it takes to regenerate a bullet
    public int maxBullets = 2; // Maximum bullets

    public static int currentBullets; // Current bullets in the gun
    private float nextFireTime = 0f; // Cooldown timer for firing

    void Start()
    {
        currentBullets = maxBullets;
        StartCoroutine(RegenerateBullets());
    }

    void Update()
    {
        if (GameMenu.isPaused || GameMenu.playerFrozen)
        {
            return;
        }

        if (Input.GetButton("Fire2") && Time.time >= nextFireTime && currentBullets > 0)
        {
            nextFireTime = Time.time + fireCooldown;
            ExplosiveShot();
        }
    }

    void ExplosiveShot()
    {
        RaycastHit hit;

        if (Physics.Raycast(FirePoint.position, FirePoint.forward, out hit, range))
        {
            // Spawn the explosion effect at the hit point
            if (ExplosionEffect != null)
            {
                GameObject explosion = Instantiate(ExplosionEffect, hit.point, Quaternion.identity);
                Destroy(explosion, 2.0f);
            }

            // Find all colliders in the explosion radius
            Collider[] colliders = Physics.OverlapSphere(hit.point, explosionRadius);
            foreach (Collider nearbyObject in colliders)
            {
                // Check if the object has an EnemyAI script
                EnemyAI enemy = nearbyObject.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        currentBullets--; // Reduce the bullet count
    }

    IEnumerator RegenerateBullets()
    {
        while (true)
        {
            yield return new WaitForSeconds(bulletRegenTime);

            if (currentBullets < maxBullets)
            {
                currentBullets++;
                Debug.Log("Bullet regenerated. Current bullets: " + currentBullets);
            }
        }
    }

    // Visualize the explosion radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(FirePoint.position, explosionRadius);
    }
}
