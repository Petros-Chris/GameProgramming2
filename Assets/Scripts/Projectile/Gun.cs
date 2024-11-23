using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject HitPoint;
    public GameObject Fire; 
    public Transform FirePoint; 
    public Transform Nozzle;
    public float range = 100f; 
    public float damage = 10f; 
    public static int magazineSize = 50;
    public float reloadTime = 3.0f; 
    public float fireRate = 0.1f; 

    public static int currentBullets; 
    private bool isReloading = false; 
    private float nextFireTime = 0f;

    void Start()
    {
        currentBullets = magazineSize;
    }

    void Update()
    {
       
        if (GameMenu.isPaused || GameMenu.playerFrozen || isReloading)
        {
            return;
        }

      
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentBullets > 0)
        {
            nextFireTime = Time.time + fireRate; 
            Shoot();
        }

    
        if (currentBullets <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
         RaycastHit hit;

    
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
            enemy.TakeDamage(damage);
        }
    }

  
    if (Fire != null)
    {
        GameObject fire = Instantiate(Fire, Nozzle.position, FirePoint.rotation);
        Destroy(fire, 1.0f);
    }

    currentBullets--; 
}

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentBullets = magazineSize; 
        isReloading = false;
        Debug.Log("Reload Complete");
    }
}
