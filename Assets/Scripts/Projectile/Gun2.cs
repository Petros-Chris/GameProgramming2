using System.Collections;
using UnityEngine;

public class Gun2 : MonoBehaviour
{
    public GameObject ExplosionEffect; 
    public Transform FirePoint; 
    public float range = 100f; 
    public float explosionRadius = 5f; 
    public float damage = 45f;
    public float fireCooldown = 5f; 
    public float bulletRegenTime = 10f; 
    public int maxBullets = 2; 

    public static int currentBullets; 
    private float nextFireTime = 0f; 

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
  
            if (ExplosionEffect != null)
            {
                GameObject explosion = Instantiate(ExplosionEffect, hit.point, Quaternion.identity);
                Destroy(explosion, 2.0f);
            }

            Collider[] colliders = Physics.OverlapSphere(hit.point, explosionRadius);
            foreach (Collider nearbyObject in colliders)
            {

                EnemyAI enemy = nearbyObject.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        currentBullets--;
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
