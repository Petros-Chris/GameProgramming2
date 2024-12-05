using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public GameObject HitPoint;
    public GameObject Fire; 
    public Transform FirePoint; 
    public Transform Nozzle;
    public float range = 100f; 
    public int ForwardVelocity = 700;
    public int launchVelocity = 100;
    public TrailRenderer bulletTrail;
    public LayerMask whatIsEnemy;
    public float bulletDamage = 5f;
    public void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = FirePoint.position;


         Vector3 randomOffset = new Vector3(
        0f,
        Random.Range(-0.2f, 0.2f), 
        Random.Range(-0.2f, 0.2f)
         );
         Vector3 rayDirection = (FirePoint.forward + randomOffset).normalized;
        float remainingRange = range;

    
       var bullet = Instantiate(bulletTrail, Nozzle.position, Quaternion.identity);
    bullet.AddPosition(Nozzle.position);
    bullet.transform.position = transform.position + (Nozzle.forward * 200);

    while (Physics.Raycast(rayOrigin, rayDirection, out hit, remainingRange))
    {
        if (((1 << hit.transform.gameObject.layer) & whatIsEnemy) != 0)
        {
            remainingRange -= hit.distance;
            rayOrigin = hit.point + rayDirection * 0.01f;
            continue;
        }

        Debug.DrawRay(rayOrigin, rayDirection * hit.distance, Color.black, 5.0f);

        if (HitPoint != null)
        {
            GameObject particle = Instantiate(HitPoint, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(particle, 1.0f);
        }

        IDamageable damageable = hit.transform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(bulletDamage);
        }

        break;
    }
    

  
    if (Fire != null)
    {
        GameObject fire = Instantiate(Fire, Nozzle.position, FirePoint.rotation);
        Destroy(fire, 1.0f);
    }

   
    }
}
