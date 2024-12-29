using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Gun : Weapon
{
    new void Start()
    {
        base.Start();
        audioPath0 = "PlayerShoot";
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
            Shoot();
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

    void Shoot()
    {
        RaycastHit hit;
        var bullet = Instantiate(bulletTrail, Nozzle.position, Quaternion.identity);
        bullet.AddPosition(Nozzle.position);
        bullet.transform.position = transform.position + (Nozzle.forward * 200);

        SoundFXManager.instance.PrepareSoundFXClip(audioPath0, transform, 0.5f);

        if (Physics.Raycast(FirePoint.position, FirePoint.forward, out hit, range))
        {
            // Debug remove for line to dissapear
            Debug.DrawRay(FirePoint.position, FirePoint.forward * hit.distance, Color.black, 5.0f);


            if (HitPoint != null)
            {
                GameObject particle = Instantiate(HitPoint, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(particle, 1.0f);
            }


            EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();
            if (enemy != null)
            {

                enemy.TakeDamage(damage, gameObject);
            }
        }

        if (Fire != null)
        {
            GameObject fire = Instantiate(Fire, Nozzle.position, FirePoint.rotation);
            Vector3 originalScale = fire.transform.localScale;
            fire.transform.parent = Nozzle.transform;
            fire.transform.localScale = originalScale;
            Destroy(fire, 1.0f);
        }
        currentBullets--;
    }

}
