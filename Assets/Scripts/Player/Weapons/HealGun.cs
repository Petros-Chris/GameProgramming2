using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HealGun : Weapon
{

    new void Start()
    {
        base.Start();
        currentBullets = magazineSize;
        audioPath0 = "BuildingUpgrade";
    }

    void Update()
    {
        if (GameMenu.Instance.isPaused || GameMenu.Instance.playerFrozen || isReloading || GameMenu.Instance.isUpdateMenuOpen)
        {
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentBullets > 0)
        {
            nextFireTime = Time.time + fireRate;
            HealLink();
        }

        if (currentBullets <= 0 && !isReloading)
        {
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.Reload(gameObject.GetComponent<Weapon>()));
        }

        if (Input.GetKeyDown(reloadKey) && !isReloading && currentBullets != magazineSize)
        {
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.Reload(gameObject.GetComponent<Weapon>()));
        }
    }

    void HealLink()
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


            IDamageable ally = hit.transform.GetComponent<IDamageable>();
            if (ally != null)
            {
                if (!ComponentManager.Instance.lockCamera)
                {
                    ally.Heal(damage * 8);
                }
                else
                {
                    ally.Heal(damage);
                }
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
