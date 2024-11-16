using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject projectile;
    public float launchVelocity = -700f;

    void Update()
    {
        // Stops player from shooting in menu
        if (GameMenu.isPaused)
        {
            return;
        }

        // Stops player from shooting in build mode
        if (GameMenu.playerFrozen)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            GameObject ball = Instantiate(projectile, transform.position, transform.rotation);

            ball.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, launchVelocity, 0));
        }
    }
}
