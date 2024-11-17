using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun1 : MonoBehaviour
{
    public GameObject projectile;
    public float launchVelocity = -700f;
    public int magazineSize = 50;
    public float reloadTime = 3.0f;
    public float fireRate = 0.5f; 
    public int pelletsPerShot = 8; 
    public float spreadAngle = 30f; 
    private int currentBullets;
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

        if (Input.GetButton("Fire1") && currentBullets > 0 && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
        else if (currentBullets <= 0 && !isReloading)
        {

            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        for (int i = 0; i < pelletsPerShot; i++)
        {

            float angle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);


            GameObject ball = Instantiate(projectile, transform.position, transform.rotation * rotation);
            ball.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, launchVelocity, 0));
        }


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
