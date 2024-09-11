using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public GameObject projectile;
    public int ForwardVelocity = 700;
    public int launchVelocity = 100;

    public void Shoot()
    {
        GameObject ball = Instantiate(projectile, transform.position, transform.rotation);

        ball.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, launchVelocity, ForwardVelocity));
    }
}
