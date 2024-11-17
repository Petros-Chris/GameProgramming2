using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject projectile;
    public float launchVelocity = -700f;
    public int magazineSize = 50; 
    public float reloadTime = 3.0f; 
    public float fireRate = 0.1f;
    private int currentBullets; 
    private bool isReloading = false; 
     private float nextFireTime = 0f;


    void Start(){
        currentBullets = magazineSize;

    }
    void Update()
    {
        // Stops player from shooting in menu or if paused or its reloading
       if (GameMenu.isPaused || GameMenu.playerFrozen || isReloading)
        {
            return;
        }


         if (Input.GetButton("Fire1") && currentBullets > 0 && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate; // Update the time for the next shot
            Shoot();
        }
        else if (currentBullets <= 0 && !isReloading)
        {
            // Start the reload coroutine when out of bullets
            StartCoroutine(Reload());
        }
    
    }
     void Shoot()
    {
        GameObject ball = Instantiate(projectile, transform.position, transform.rotation);
        ball.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, launchVelocity, 0));
        currentBullets--; 
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentBullets = magazineSize; 
        isReloading = false;
    }
}
    

