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
    public LayerMask whatIsBuilding;
    public LayerMask whatIsAlly;
    public float bulletDamage = 5f;
    public void Shoot()
    {
        RaycastHit hit;
        Vector3 rayOrigin = FirePoint.position;
        Vector3 randomOffset = new Vector3(
                0f,
                Random.Range(-0.05f, 0.05f),
                Random.Range(-0.05f, 0.05f)
            );
        Vector3 rayDirection = (FirePoint.forward + randomOffset).normalized;
        float remainingRange = range;

        var bullet = Instantiate(bulletTrail, Nozzle.position, Quaternion.identity);
        bullet.AddPosition(Nozzle.position);
        bullet.transform.position = transform.position + (Nozzle.forward * 200);

        while (Physics.Raycast(rayOrigin, rayDirection, out hit, remainingRange))
        {

            if (((1 << hit.transform.gameObject.layer) & (whatIsBuilding | whatIsAlly | LayerMask.GetMask("gateWall"))) != 0)
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

            EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(bulletDamage, gameObject);
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
