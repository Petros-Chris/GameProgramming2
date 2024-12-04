using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGun : MonoBehaviour
{
    public GameObject HitPoint;
    public GameObject Fire; 
    public Transform FirePoint; 
    public Transform Nozzle;
    public float range = 100f; 
    public int ForwardVelocity = 700;
    public int launchVelocity = 100;
    public TrailRenderer bulletTrail;
    public void Shoot()
    {
       RaycastHit hit;
    
        var bullet = Instantiate(bulletTrail, Nozzle.position, Quaternion.identity);
        bullet.AddPosition(Nozzle.position);
        {
            bullet.transform.position = transform.position + (Nozzle.forward * 200);
        }
    if (Physics.Raycast(FirePoint.position, FirePoint.forward, out hit, range))
    {
        // Debug remove for line to dissapear
        Debug.DrawRay(FirePoint.position, FirePoint.forward * hit.distance, Color.black, 5.0f);


        if (HitPoint != null)
        {
            GameObject particle = Instantiate(HitPoint, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(particle, 1.0f); 
        }


        IDamageable damageable = hit.transform.GetComponent<IDamageable>();
        
        if (damageable != null){

            damageable.TakeDamage(1f);
        }
           
        
    }

  
    if (Fire != null)
    {
        GameObject fire = Instantiate(Fire, Nozzle.position, FirePoint.rotation);
        Destroy(fire, 1.0f);
    }

   
    }
}
