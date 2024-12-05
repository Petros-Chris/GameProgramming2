using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour
{
    public GameObject HitPoint;
    public GameObject Fire; 
    public Transform FirePoint; 
    public Transform Nozzle;
    public float range = 100f; 
    public float damage = 4f; 
    public static int magazineSize = 50;
    public float reloadTime = 3.0f; 
    public float fireRate = 0.1f; 

    public static int currentBullets; 
    private bool isReloading = false; 
    private float nextFireTime = 0f;

    public TrailRenderer bulletTrail;

    private KeyCode reloadKey = KeyCode.R;

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

        if(Input.GetKeyDown(reloadKey) && !isReloading && currentBullets != magazineSize){
            StartCoroutine(Reload());
        }
    }

    void Shoot()
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


        EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
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

    IEnumerator Reload()
    {
        currentBullets = 0;
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentBullets = magazineSize; 
        isReloading = false;
        Debug.Log("Reload Complete");
    }
}
