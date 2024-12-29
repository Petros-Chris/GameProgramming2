using UnityEngine;

public class RocketGun : Weapon
{
    public GameObject ExplosionEffect;
    public float explosionRadius = 5f;
    public string audioPath2;
    new void Start()
    {
        base.Start();
        currentBullets = magazineSize;
        audioPath0 = "ExplosionGun";
        audioPath2 = "BarrelExploding";
    }

    void Update()
    {
        if (GameMenu.Instance.isPaused || GameMenu.Instance.playerFrozen || isReloading || GameMenu.Instance.isInGameMenuOpen)
        {
            return;
        }

        if (attack.IsPressed() && Time.time >= nextFireTime && currentBullets > 0)
        {
            nextFireTime = Time.time + fireRate;
            ExplosiveShot();
        }


        if (currentBullets <= 0 && !isReloading)
        {
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.Reload(gameObject.GetComponent<Weapon>()));
        }

        if (reloadKey.triggered && !isReloading && currentBullets != magazineSize)
        {
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.Reload(gameObject.GetComponent<Weapon>()));
        }
    }

    void ExplosiveShot()
    {
        SoundFXManager.instance.PrepareSoundFXClip(audioPath2, transform, 0.5f);
        RaycastHit hit;
        if (Physics.Raycast(FirePoint.position, FirePoint.forward, out hit, range))
        {

            if (ExplosionEffect != null)
            {
                SoundFXManager.instance.PrepareSoundFXClip(audioPath0, transform, 0.5f);
                GameObject explosion = Instantiate(ExplosionEffect, hit.point, Quaternion.identity);
                Destroy(explosion, 2.0f);
            }

            Collider[] colliders = Physics.OverlapSphere(hit.point, explosionRadius);
            foreach (Collider nearbyObject in colliders)
            {

                EnemyAI enemy = nearbyObject.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, gameObject);
                }
            }
        }
        currentBullets--;
    }
}
